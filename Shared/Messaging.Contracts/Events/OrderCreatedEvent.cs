namespace Messaging.Contracts.Events;

public class OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public List<OrderItemEvent> Items { get; init; } = [];
}

public record OrderItemEvent
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}