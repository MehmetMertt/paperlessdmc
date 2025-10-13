using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaperlessREST.Infrastructure
{
    public class OcrWorker : BackgroundService
    {
        private readonly ILogger<OcrWorker> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public OcrWorker(ILogger<OcrWorker> logger)
        {
            _logger = logger;

            // establish rabbitmq connection
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // declares queue (has to be the same as the of the sender)
            _channel.QueueDeclare(queue: "document_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        private void ProcessFile(string message)
        {
            _logger.LogInformation("Processing file (simulated OCR): {message}", message);

            // todo here real ocr processing code and not simulation + use exception handling here
            Thread.Sleep(2000); // simulates processing by sleeping :P

            _logger.LogInformation("File processed successfully: {message}", message);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OCR Worker started. Listening for messages...");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("OCR Worker received message: {message}", message);

                ProcessFile(message); // simulates processing
            };

            _channel.BasicConsume(queue: "document_queue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
