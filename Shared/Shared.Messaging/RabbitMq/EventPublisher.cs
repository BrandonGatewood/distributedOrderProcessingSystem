using System.Text;
using System.Text.Json;
using Shared.Messaging.Interfaces;
using Shared.Messaging.Constants;
using RabbitMQ.Client;

namespace Shared.Messaging.RabbitMq;

public class EventPublisher(IRabbitMqConnection connection) : IEventPublisher
{
    private readonly IRabbitMqConnection _connection = connection;

    public async Task PublishAsync<T>(string exchange, string routingKey, T message)
    {
        // Create channel
        await using var channel = await _connection.Connection.CreateChannelAsync();

        // Declare message exchange
        await channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: ExchangeType.Topic,
            durable: true
        );

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        // Publish the message
        await channel.BasicPublishAsync(
            exchange: RabbitMqConstants.OrderExchange,
            routingKey: routingKey,
            body: body
        );
    }
}