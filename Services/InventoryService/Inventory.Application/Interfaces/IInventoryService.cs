using Shared.Contracts.Events;

namespace Inventory.Application.Interfaces;

public interface IInventoryService
{
    Task<bool> ProcessInventoryAsync(OrderCreatedEvent order, CancellationToken cancellationToken);
}