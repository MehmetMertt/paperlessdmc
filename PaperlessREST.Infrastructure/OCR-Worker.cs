using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Minio;
using Minio.DataModel.Args;
using PaperlessREST.DataAccess;
using ImageMagick;
using System.Text.Json;

namespace PaperlessREST.Infrastructure
{
    public class OcrWorker : BackgroundService
    {
        private readonly ILogger<OcrWorker> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName = "document_queue";
        private readonly IMinioClient _minioClient;
        private readonly TesseractService _tesseract;

        public OcrWorker(ILogger<OcrWorker> logger)
        {
            _logger = logger;
            _minioClient = new MinioClient()
                .WithEndpoint("minio:9000")
                .WithCredentials("minioadmin", "minioadmin")
                .Build();

            _tesseract = new TesseractService();
        }

        private void ConnectToRabbitMq()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
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
                    var job = JsonSerializer.Deserialize<OcrJobMessage>(message);
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

        private async Task ProcessOcrJobAsync(OcrJobMessage job)
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

            // Optionally upload OCR text back to MinIO
            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"{job.DocumentId}_ocr.txt")
                    .WithFileName(txtPath)
                    .WithContentType("text/plain")
            );
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    public class OcrJobMessage
    {
        public string DocumentId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
    }
}
