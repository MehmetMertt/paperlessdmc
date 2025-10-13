using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaperlessREST.Application;
using RabbitMQ.Client;
using System.Text;

public class RabbitMqService : IMessageQueue, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel; // lightweight communication option for sending and receiving messages
    private readonly string _queueName = "document_queue"; // queues have names
    private readonly ILogger<RabbitMqService> _logger;

    public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
    {
        _logger = logger;

        var host = configuration["RabbitMQ:Host"];
        var username = configuration["RabbitMQ:Username"];
        var password = configuration["RabbitMQ:Password"];

        var factory = new ConnectionFactory()
        {
            HostName = host,
            UserName = username,
            Password = password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false); // creates the queue
    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: _queueName, body: body); // mehtod to send the message
    }

    public void Dispose() // frees resources, and closes channels and connections
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
