using RabbitMQ.Client;
using Shared.Interfaces;
using System.Text.Json;
using System.Text;

public class EventPublisher : IEventPublisher
{
    private readonly IModel _channel;
    private const string ExchangeName = "SmartCharging";

    public EventPublisher(IConnection connection)
    {
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
    }

    public Task PublishAsync<TEvent>(TEvent eventMessage)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var message = JsonSerializer.Serialize(eventMessage, options);
        var body = Encoding.UTF8.GetBytes(message);

        var routingKey = typeof(TEvent).Name;
        _channel.BasicPublish(exchange: ExchangeName, routingKey: routingKey, basicProperties: null, body: body);

        return Task.CompletedTask;
    }
}
