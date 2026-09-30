using OrderService.Application.Interfaces;
using Shared.Contracts.Events;
using Shared.Messaging.Constants;
using Shared.Messaging.Interfaces;

namespace OrderService.Api.Workers;

public class OrderEventWorker(ILogger<OrderEventWorker> logger, IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private readonly ILogger<OrderEventWorker> _logger = logger;

    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order event worker starting...");

        using var scope = _serviceScopeFactory.CreateScope();

        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

        var reservedTask = eventConsumer.ConsumeAsync<InventoryReservedEvent>(
            exchange: RabbitMqConstants.InventoryExchange,
            queue: RabbitMqConstants.OrderInventoryReservedQueue,
            routingKey: RabbitMqConstants.InventoryReservedRoutingKey,
            message: default!,
            callback: async inventoryReserved =>
            {
                _logger.LogInformation("Order inventory reserved success.");

                using var scope = _serviceScopeFactory.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderApplicationService>(); 

                await orderService.CompleteOrderAsync(inventoryReserved.OrderId);
            },
            cancellationToken: stoppingToken
        );

        var failedTask = eventConsumer.ConsumeAsync<InventoryFailedEvent>(
            exchange: RabbitMqConstants.InventoryExchange,
            queue: RabbitMqConstants.OrderInventoryFailedQueue,
            routingKey: RabbitMqConstants.InventoryFailedRoutingKey,
            message: default!,
            callback: async inventoryFailed =>
            {
                _logger.LogInformation("Order inventory reserved failed.");

                using var scope = _serviceScopeFactory.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderApplicationService>(); 

                await orderService.CancelOrderAsync(inventoryFailed.OrderId);
            },
            cancellationToken: stoppingToken
        );

        await Task.WhenAll(reservedTask, failedTask); 
    }
}