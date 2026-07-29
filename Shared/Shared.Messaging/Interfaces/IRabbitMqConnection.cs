using RabbitMQ.Client;

namespace Shared.Messaging.Interfaces;

public interface IRabbitMqConnection
{
    IConnection Connection { get; }
}