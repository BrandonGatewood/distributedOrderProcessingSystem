namespace OrderService.Domain.Entities;

public class OrderItem
{
    public required Guid Id { get; set; }
    public required Guid OrderId { get; set; }
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity; 
}