namespace Shared.Messaging.Interfaces;

public interface IEventConsumer
{
    Task ConsumeAsync<T>(string exchange, string queue, string routingKey, T message, Func<T, Task> callback, CancellationToken cancellationToken);
}