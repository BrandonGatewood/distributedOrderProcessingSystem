using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Shared.Contracts.Events;

namespace Inventory.Application.Services;

public class InventoryService(IInventoryRepository inventoryRepository) : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    
   public async Task<bool> ProcessInventoryAsync(OrderCreatedEvent order, CancellationToken cancellationToken)
    {
        var items = order.Items
            .Select(x => new InventoryItem
            {
                Id = x.ProductId,
                Quantity = x.Quantity
            })
            .ToList();

        return await _inventoryRepository.TryReserveOrderAsync(items, cancellationToken);
    } 
}