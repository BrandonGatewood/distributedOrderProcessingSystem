using System.Text;
using System.Text.Json;
using Shared.Messaging.Interfaces;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace Shared.Messaging.RabbitMq;

public class EventPublisher(ILogger<EventPublisher> logger, IRabbitMqConnection connection) : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger = logger;
    private readonly IRabbitMqConnection _connection = connection;

    public async Task PublishAsync<T>(string exchange, string routingKey, T message)
    {
        try
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
                exchange: exchange,
                routingKey: routingKey,
                body: body
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish event to exchange {Exchange} with routing key {RoutingKey}",
                exchange,
                routingKey
            );

            throw;
        }
    }
}