using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Messaging.Interfaces;
using Shared.Messaging.Configuration;

namespace Shared.Messaging.RabbitMq;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConnection _connection;

    public RabbitMqConnection(IOptions<RabbitMqSettings> options)
    {
        var settings = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,

            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public IConnection Connection
    {
        get
        {
            return _connection;
        }
    }

    public void Dispose()
    {
        if(_connection.IsOpen)
            _connection.CloseAsync().GetAwaiter().GetResult();

        _connection.Dispose();

        GC.SuppressFinalize(this);
    }
}