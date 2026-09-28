using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IInventoryRepository
{
    Task<bool> TryReserveOrderAsync(IEnumerable<InventoryItem> items, CancellationToken cancellationToken);
}