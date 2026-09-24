namespace Inventory.Domain.Entities;

public class InventoryItem
{
    public required Guid Id { get; set; }
    public required Guid ProductId { get; set; }
    public required int Quantity { get; set; }
}