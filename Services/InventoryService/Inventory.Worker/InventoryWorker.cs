using Inventory.Application.Interfaces;
using Shared.Contracts.Events;
using Shared.Messaging.Constants;
using Shared.Messaging.Interfaces;

namespace Inventory.Worker;

public class InventoryWorker(ILogger<InventoryWorker> logger, IEventConsumer eventConsumer, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly ILogger<InventoryWorker> _logger = logger;
    private readonly IEventConsumer _eventConsumer = eventConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Inventory worker starting...");
        await _eventConsumer.ConsumeAsync<OrderCreatedEvent>(
            exchange: RabbitMqConstants.OrderExchange,
            queue: RabbitMqConstants.InventoryQueue,
            routingKey: RabbitMqConstants.OrderCreatedRoutingKey,
            message: default!,
            callback: async orderCreated =>
            {
                _logger.LogInformation(
                    "Processing inventory for order {OrderId}",
                    orderCreated.OrderId
                );

                using var scope = _serviceScopeFactory.CreateScope();
                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                var succeeded = await inventoryService.ProcessInventoryAsync(orderCreated, cancellationToken);

                if (succeeded)
                {
                     _logger.LogInformation(
                        "Inventory reserved successfully for order {OrderId}",
                        orderCreated.OrderId
                    );

                    var message = new InventoryReservedEvent
                    {
                        OrderId = orderCreated.OrderId
                    };

                    await eventPublisher.PublishAsync(
                        RabbitMqConstants.InventoryExchange,
                        RabbitMqConstants.InventoryReservedRoutingKey,
                        message);

                    _logger.LogInformation(
                        "Published InventoryReservedEvent for order {OrderId}",
                        orderCreated.OrderId
                    );
                }
                else
                {
                    _logger.LogWarning(
                        "Inventory reservation failed for order {OrderId}: insufficient inventory",
                        orderCreated.OrderId
                    );

                    var message = new InventoryFailedEvent
                    {
                        OrderId = orderCreated.OrderId,
                        Reason = "Insufficient inventory"
                    };

                    await eventPublisher.PublishAsync(
                        RabbitMqConstants.InventoryExchange,
                        RabbitMqConstants.InventoryFailedRoutingKey,
                        message);

                    _logger.LogInformation(
                        "Published InventoryFailedEvent for order {OrderId}",
                        orderCreated.OrderId
                    );
                }
            },
            cancellationToken: cancellationToken 
        );
    }
}
