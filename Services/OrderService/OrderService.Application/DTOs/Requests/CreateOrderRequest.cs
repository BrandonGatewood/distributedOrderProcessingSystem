namespace OrderService.Application.DTOs.Requests;

public class CreateOrderRequest
{
    public required Guid UserId { get; set; }
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
}