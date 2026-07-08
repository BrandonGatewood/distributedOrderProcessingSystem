namespace OrderService.Application.DTOs.Requests;

public class CreateOrderItemsRequest
{
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }
}