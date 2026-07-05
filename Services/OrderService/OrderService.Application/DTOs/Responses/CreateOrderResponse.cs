namespace OrderService.Application.DTOs.Responses;

public class CreateOrderResponse
{
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}