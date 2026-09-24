namespace Shared.Contracts.Events;

public class InventoryReservedEvent 
{
    public Guid OrderId { get; init; }
}