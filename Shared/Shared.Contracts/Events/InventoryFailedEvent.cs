namespace Shared.Contracts.Events;

public class InventoryFailedEvent 
{
    public required Guid OrderId { get; init; }
    public required string Reason { get; init; }
}