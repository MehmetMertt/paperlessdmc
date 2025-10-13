using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaperlessREST.Infrastructure
{
    public class OcrWorker : BackgroundService
    {
        private readonly ILogger<OcrWorker> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName = "document_queue";

        public OcrWorker(ILogger<OcrWorker> logger)
        {
            _logger = logger;
        }

        private void ConnectToRabbitMqWithRetry() // in case a connection establishment should not be possible try again a few times
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq", // <- docker-servicename
                UserName = "user",
                Password = "password"
            };

            int maxRetries = 10;
            for (int i = 1; i <= maxRetries; i++)
            {
                try
                {
                    _logger.LogInformation("Attempting to connect to RabbitMQ... try {Attempt}", i);
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
                    _logger.LogInformation("Connected to RabbitMQ successfully!");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ not ready yet (attempt {Attempt}/{Max})", i, maxRetries);
                    Thread.Sleep(3000); // wait 3 seconds and retry
                }
            }

            throw new Exception("Could not connect to RabbitMQ after multiple attempts.");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConnectToRabbitMqWithRetry();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Processing message from RabbitMQ: {Message}", message);

                // here "simulate" ocr processing
                Thread.Sleep(2000);
                _logger.LogInformation("Processed document successfully: {Message}", message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
