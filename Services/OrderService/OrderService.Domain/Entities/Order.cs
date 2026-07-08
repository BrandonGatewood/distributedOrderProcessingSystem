using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order
{
    public required Guid Id { get; set;}
    public required Guid UserId { get; set;}
    public required List<OrderItem> OrderItems = []; 
    public required decimal TotalAmount { get; set; }
    public required OrderStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}