using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductIdAsync(Guid productId);
    Task UpdateAsync(InventoryItem inventoryItem);
}