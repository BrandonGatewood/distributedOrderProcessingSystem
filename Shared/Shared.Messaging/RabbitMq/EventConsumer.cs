using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging.Interfaces;

namespace Shared.Messaging.RabbitMq;

public class EventConsumer(IRabbitMqConnection connection) : IEventConsumer
{
    private readonly IRabbitMqConnection _connection = connection;

    public async Task ConsumeAsync<T>(string exchange, string queue, string routingKey, T message, Func<T, Task> callback, CancellationToken cancellationToken)
    {
        // Create channel
        await using var channel = await _connection.Connection.CreateChannelAsync();

        // Declare message exchange
        await channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken
        );

        // Create queue
        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        // Connect queue to exchange
        await channel.QueueBindAsync(
            queue: queue,
            exchange: exchange,
            routingKey: routingKey,
            cancellationToken: cancellationToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        // Message recieved
        consumer.ReceivedAsync += async (_, args) =>
        {
            var body = args.Body.ToArray();

            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<T>(json);

            if (message != null)
            {
                await callback(message);
            }

            // Successfully processed message
            await channel.BasicAckAsync(
                deliveryTag: args.DeliveryTag,
                multiple: false
            );
        };

        // Start listening
        await channel.BasicConsumeAsync(
            queue: queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken
        );


        // Keep the BackgroundService alive
        await Task.Delay(
            Timeout.Infinite,
            cancellationToken
        );  
    }
}