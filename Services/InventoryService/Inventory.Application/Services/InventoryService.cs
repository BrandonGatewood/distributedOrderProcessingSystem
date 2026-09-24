using Inventory.Application.Interfaces;
using Shared.Contracts.Events;
using Shared.Messaging.Constants;
using Shared.Messaging.Interfaces;

namespace Inventory.Application.Services;

public class InventoryService(IInventoryRepository inventoryRepository, IEventPublisher eventPublisher) : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    
    public async Task ProcessInventory(OrderCreatedEvent order)
    {
        // read db
        foreach(var item in order.Items)
        {
            var inventoryItem = await _inventoryRepository
                .GetByProductIdAsync(item.ProductId);

            if (inventoryItem == null)
            {
                await _eventPublisher.PublishAsync(
                    RabbitMqConstants.InventoryExchange,
                    RabbitMqConstants.InventoryFailedRoutingKey,
                    new InventoryFailedEvent 
                    {
                        OrderId = order.OrderId,
                        Reason = $"Product {item.ProductId} was not found in inventory."
                    }
                );

                return;
            }

            if (inventoryItem.Quantity < item.Quantity)
            {
                await _eventPublisher.PublishAsync(
                    RabbitMqConstants.InventoryExchange,
                    RabbitMqConstants.InventoryFailedRoutingKey,
                    new InventoryFailedEvent
                    {
                        OrderId = order.OrderId,
                        Reason = $"Insufficient stock for product {item.ProductId}."
                    }
                );

                return;
            } 
        }

        foreach (var item in order.Items)
        {
            var inventoryItem = await _inventoryRepository
                .GetByProductIdAsync(item.ProductId);

            inventoryItem!.Quantity -= item.Quantity;

            await _inventoryRepository.UpdateAsync(inventoryItem);
        }

        // Tell Order Service inventory was successfully reserved
        await _eventPublisher.PublishAsync(
            RabbitMqConstants.InventoryExchange,
            RabbitMqConstants.InventoryReservedRoutingKey,
            new InventoryReservedEvent
            {
                OrderId = order.OrderId
            }
        );
    }
}