using System.Text;
using System.Text.Json;
using OrderService.Application.Interfaces;
using Messaging.Contracts.RabbitMq;
using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging;

public class RabbitMqPublisher(IRabbitMqConnection connection) : IEventPublisher
{
    private readonly IRabbitMqConnection _connection = connection;

    public async Task PublishAsync<T>(string routingKey, T message)
    {
        // Create channel
        await using var channel = await _connection.Connection.CreateChannelAsync();

        // Declare message exchange
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.OrderExchange,
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