namespace OrderService.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string routingKey, T message);
}