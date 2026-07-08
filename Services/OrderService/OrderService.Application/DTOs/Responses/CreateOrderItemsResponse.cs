namespace OrderService.Application.DTOs.Responses;

public class CreateOrderItemsResponse
{
    public required string ProductName { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity; 
}