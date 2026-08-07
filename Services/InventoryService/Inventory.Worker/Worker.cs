using Inventory.Application.Interfaces;
using Shared.Contracts.Events;
using Shared.Messaging.Constants;
using Shared.Messaging.Interfaces;

namespace Inventory.Worker;

public class Worker(ILogger<Worker> logger, IEventConsumer eventConsumer, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IEventConsumer _eventConsumer = eventConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                    "Received order {OrderId}",
                    orderCreated.OrderId
                );

                using var scope = _serviceScopeFactory.CreateScope();
                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

                await inventoryService.ProcessInventory(orderCreated);

            },
            cancellationToken: stoppingToken
        );
    }
}
