using ImageMagick;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PaperlessREST.Application.DTOs;
using PaperlessREST.DataAccess.Service;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaperlessREST.OcrWorker.Services
{
    public class OCRWorker : BackgroundService
    {
        private readonly ILogger<OCRWorker> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName = "document_queue";
        private readonly IMinioClient _minioClient;
        private readonly TesseractService _tesseract;
        private readonly GenAiService _genai;
        private readonly IMetaDataService _metaDataService;

        public OCRWorker(ILogger<OCRWorker> logger, GenAiService genai, IMetaDataService metaDataService)
        {
            _logger = logger;
            _minioClient = new MinioClient()
                .WithEndpoint("minio:9000")
                .WithCredentials("minioadmin", "minioadmin")
                .Build();

            _tesseract = new TesseractService();
            _genai = genai;
            _metaDataService = metaDataService;
        }

        private void ConnectToRabbitMq()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq", // <- docker-servicename
                UserName = "user",
                Password = "password"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConnectToRabbitMq();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received OCR job: {Message}", message);

                try
                {
                    var job = JsonSerializer.Deserialize<OCRJobDTO>(message);
                    await ProcessOcrJobAsync(job);
                    _logger.LogInformation("OCR job finished for {Id}", job.DocumentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OCR job: {Message}", message);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessOcrJobAsync(OCRJobDTO job)
        {
            string bucketName = "paperless-data";
            string tempPdf = Path.Combine(Path.GetTempPath(), $"{job.DocumentId}.pdf");
            string txtPath = Path.Combine(Path.GetTempPath(), $"{job.DocumentId}_ocr.txt");

            // download pdf from minio
            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(job.FileName)
                    .WithFile(tempPdf)
            );

            // convert pdf to pngs
            var images = new List<string>();
            using (var pdfImages = new MagickImageCollection(tempPdf))
            {
                int i = 0;
                foreach (var page in pdfImages)
                {
                    var imagePath = Path.Combine(Path.GetTempPath(), $"{job.DocumentId}_page{i}.png");
                    page.Write(imagePath);
                    images.Add(imagePath);
                    i++;
                }
            }

            // run ocr
            var sb = new StringBuilder();
            foreach (var imgPath in images)
            {
                using var imgStream = File.OpenRead(imgPath);
                var text = _tesseract.ExtractTextFromImage(imgStream);
                sb.AppendLine(text);
            }

            await File.WriteAllTextAsync(txtPath, sb.ToString()); // saves the ocr result

            // upload analysed ocr text back to minio
            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"{job.DocumentId}_ocr.txt")
                    .WithFileName(txtPath)
                    .WithContentType("text/plain")
            );

            // send ocr text to genai
            string summary = "";
            try
            {
                summary = await _genai.SummarizeAsync(sb.ToString());
                _logger.LogInformation("Summary generated: {Summary}", summary.Substring(0, Math.Min(200, summary.Length)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Summary generation failed");
            }

            // save summary in database
            try
            {
                var meta = _metaDataService.GetMetaDataByGuid(Guid.Parse(job.DocumentId));

                if (meta != null)
                {
                    meta.Summary = summary;
                    _metaDataService.UpdateMetadata(meta);
                    _logger.LogInformation("Summary saved to database for {Id}", job.DocumentId);
                }
                else
                {
                    _logger.LogWarning("Could not find metadata for {Id}", job.DocumentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save summary to database for {Id}", job.DocumentId);
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