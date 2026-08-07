using Shared.Contracts.Events;

namespace Inventory.Application.Interfaces;

public interface IInventoryService
{
    Task ProcessInventory(OrderCreatedEvent order);
}