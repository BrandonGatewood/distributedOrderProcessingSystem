namespace Inventory.Domain.Entities;

public class InventoryItem
{
    public required Guid ProductId { get; set; }
    public required int Quantity { get; set; }
}