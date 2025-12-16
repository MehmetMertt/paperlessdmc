using ImageMagick;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PaperlessREST.Application.DTOs;
using PaperlessREST.DataAccess.Service;
using PaperlessREST.Domain.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PaperlessREST.OcrWorker.Services
{
    public class OCRWorker : BackgroundService
    {
        private const string QueueName = "document_queue";
        private const string BucketName = "paperless-data";
        private const double DuplicateThreshold = 0.9;

        private readonly ILogger<OCRWorker> _logger;
        private readonly IMinioClient _minio;
        private readonly IMetaDataService _metaDataService;
        private readonly IDocumentSimilarityService _similarityService;
        private readonly TesseractService _tesseract;
        private readonly GenAiService _genAi;
        private readonly ElasticsearchService _elasticsearch;

        private IConnection? _connection;
        private IModel? _channel;

        public OCRWorker(ILogger<OCRWorker> logger, IMetaDataService metaDataService, IDocumentSimilarityService similarityService, GenAiService genAi, ElasticsearchService elasticsearch)
        {
            _logger = logger;
            _minio = new MinioClient().WithEndpoint("minio:9000").WithCredentials("minioadmin", "minioadmin").Build();
            _metaDataService = metaDataService;
            _similarityService = similarityService;
            _genAi = genAi;
            _elasticsearch = elasticsearch;
            _tesseract = new TesseractService();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConnectToRabbitMq();

            var consumer = new EventingBasicConsumer(_channel!);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var job = JsonSerializer.Deserialize<OCRJobDTO>(message) ?? throw new InvalidOperationException("Invalid OCR job payload");

                    await ProcessOcrJobAsync(job, stoppingToken);
                    _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OCR job failed");
                    _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel!.BasicConsume(QueueName, autoAck: false, consumer);
            return Task.CompletedTask;
        }

        private void ConnectToRabbitMq()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "user",
                Password = "password",
                RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
            };

            const int maxRetries = 10;
            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    _logger.LogInformation("Connecting to RabbitMQ (attempt {Attempt})", attempt);

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    _channel.QueueDeclare(
                        queue: QueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false
                    );

                    _logger.LogInformation("Connected to RabbitMQ");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ not available yet");

                    if (attempt >= maxRetries)
                        throw;

                    Thread.Sleep(3000);
                }
            }
        }

        private async Task ProcessOcrJobAsync(OCRJobDTO job, CancellationToken ct)
        {
            var documentId = Guid.Parse(job.DocumentId);
            var tempDir = Path.Combine(Path.GetTempPath(), documentId.ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var pdfPath = await DownloadPdfAsync(job, tempDir, ct);
                var images = ConvertPdfToImages(pdfPath, tempDir);
                var ocrText = RunOcr(images);

                if (string.IsNullOrWhiteSpace(ocrText))
                {
                    _logger.LogWarning("Empty OCR result for {Id}", documentId);
                    return;
                }

                var meta = _metaDataService.GetMetaDataByGuid(documentId) ?? throw new InvalidOperationException($"Metadata not found for {documentId}");
                /*
                if (IsDuplicate(meta, ocrText))
                    return;
                */

                var summary = await GenerateSummaryAsync(ocrText);

                // meta.OcrText = ocrText;
                meta.Summary = summary;
                _metaDataService.UpdateMetadata(meta);

                _logger.LogInformation("OCR + summary stored for {Id}", documentId);

                string elasticFilename = job.FileName.Replace(job.DocumentId + "_", "");

                await _elasticsearch.IndexDocumentAsync(Guid.Parse(job.DocumentId), elasticFilename, ocrText, job.FileType);
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }

        private async Task<string> DownloadPdfAsync(OCRJobDTO job, string tempDir, CancellationToken ct)
        {
            var pdfPath = Path.Combine(tempDir, "document.pdf");

            await _minio.GetObjectAsync(new GetObjectArgs().WithBucket(BucketName).WithObject(job.FileName).WithFile(pdfPath), ct);

            return pdfPath;
        }

        private static IReadOnlyList<string> ConvertPdfToImages(string pdfPath, string tempDir)
        {
            var images = new List<string>();

            using var pdf = new MagickImageCollection(pdfPath);
            for (var i = 0; i < pdf.Count; i++)
            {
                var imgPath = Path.Combine(tempDir, $"page_{i}.png");
                pdf[i].Write(imgPath);
                images.Add(imgPath);
            }

            return images;
        }

        private string RunOcr(IEnumerable<string> images)
        {
            var sb = new StringBuilder();

            foreach (var img in images)
            {
                using var stream = File.OpenRead(img);
                sb.AppendLine(_tesseract.ExtractTextFromImage(stream));
            }

            return sb.ToString();
        }
        /*
        private bool IsDuplicate(MetaData meta, string ocrText)
        {
            var existingDocs = _metaDataService.GetAllMetaData().Where(d => d.Id != meta.Id && !string.IsNullOrWhiteSpace(d.OcrText));

            foreach (var doc in existingDocs)
            {
                var similarity = _similarityService.CalculateSimilarity(ocrText, doc.OcrText!);
                if (similarity >= DuplicateThreshold)
                {
                    meta.IsDuplicate = true;
                    meta.Summary = "Duplicate document detected";
                    _metaDataService.UpdateMetadata(meta);

                    _logger.LogWarning("Duplicate detected: {NewId} ~ {ExistingId} ({Similarity:P})", meta.Id, doc.Id, similarity);

                    return true;
                }
            }
            return false;
        }
        */
        private async Task<string> GenerateSummaryAsync(string text)
        {
            try
            {
                return await _genAi.SummarizeAsync(text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Summary generation failed");
                return string.Empty;
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
