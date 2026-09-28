using OrderService.Application.Interfaces;
using Shared.Contracts.Events;
using Shared.Messaging.Constants;
using Shared.Messaging.Interfaces;

namespace OrderService.Api.Workers;

public class OrderEventWorker(ILogger<OrderEventWorker> logger, IEventConsumer eventConsumer, IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private readonly ILogger<OrderEventWorker> _logger = logger;
    private readonly IEventConsumer _eventConsumer = eventConsumer;

    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order event worker starting...");

        var reservedTask = _eventConsumer.ConsumeAsync<InventoryReservedEvent>(
            exchange: RabbitMqConstants.InventoryExchange,
            queue: RabbitMqConstants.OrderInventoryReservedQueue,
            routingKey: RabbitMqConstants.InventoryReservedRoutingKey,
            message: default!,
            callback: async inventoryReserved =>
            {
                _logger.LogInformation("Order inventory reserved success.");
            },
            cancellationToken: stoppingToken
        );

        var failedTask = _eventConsumer.ConsumeAsync<InventoryFailedEvent>(
            exchange: RabbitMqConstants.InventoryExchange,
            queue: RabbitMqConstants.OrderInventoryFailedQueue,
            routingKey: RabbitMqConstants.InventoryFailedRoutingKey,
            message: default!,
            callback: async inventoryFailed =>
            {
                _logger.LogInformation("Order inventory reserved failed.");
            },
            cancellationToken: stoppingToken
        );

        await Task.WhenAll(reservedTask, failedTask); 
    }
}