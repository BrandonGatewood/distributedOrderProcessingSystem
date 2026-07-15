using RabbitMQ.Client;

namespace OrderService.Infrastructure.Messaging;

public interface IRabbitMqConnection
{
    IConnection Connection { get; }
}