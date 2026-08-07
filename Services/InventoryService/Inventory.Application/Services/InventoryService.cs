using Inventory.Application.Interfaces;
using Shared.Contracts.Events;

namespace Inventory.Application.Services;

public class InventoryService() : IInventoryService
{
    public async Task ProcessInventory(OrderCreatedEvent order)
    {
        // read db
        foreach(var item in order.Items)
        {
            // get inventory from db

            // check if enough inventory
            // return if not enough
        }

        // write db

        // return success
    }
}