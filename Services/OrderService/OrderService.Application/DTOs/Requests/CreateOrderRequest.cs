namespace OrderService.Application.DTOs.Requests;

public class CreateOrderRequest
{
    public required Guid UserId { get; set; }
    public required List<CreateOrderItemsRequest> OrderItems { get; set; }
}