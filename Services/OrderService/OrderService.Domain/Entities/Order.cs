namespace OrderService.Domain.Entities;

public class Order
{
    public required Guid Id { get; set;}
    public required Guid UserId { get; set;}
    public required Guid ProductId { get; set;}
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}