using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infrastructure.EventListeners;

public abstract class BaseEventListener<TEvent> : BackgroundService
{
    private readonly IModel _channel;
    private readonly string _queueName;
    protected readonly IServiceProvider _serviceProvider;
    private string ExchangeName = "";

    protected BaseEventListener(IConfiguration conifiguration, IConnection connection, IServiceProvider serviceProvider, string queueName)
    {
        ExchangeName = conifiguration["RabbitMQ:ExchangeName"] ?? string.Empty;

        _channel = connection.CreateModel();
        _queueName = queueName;
        _serviceProvider = serviceProvider;

        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);

        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: _queueName, exchange: ExchangeName, routingKey: typeof(TEvent).Name);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set up RabbitMQ consumer to listen on the event
        var consumer = new EventingBasicConsumer(_channel);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        consumer.Received += async (model, ea) =>
        {
            // Deserialize the message
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventMessage = JsonSerializer.Deserialize<TEvent>(message, options);

            // Handle the event
            if (eventMessage != null)
            {
                await ProcessEventAsync(eventMessage);
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    // Abstract method for processing the event, to be implemented by child classes
    public abstract Task ProcessEventAsync(TEvent eventMessage);

    public override void Dispose()
    {
        _channel.Close();
        base.Dispose();
    }
}

