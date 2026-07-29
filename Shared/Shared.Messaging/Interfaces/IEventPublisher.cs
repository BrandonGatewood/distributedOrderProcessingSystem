namespace Shared.Messaging.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string routingKey, T message);
}