using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs.Responses;

public class CreateOrderResponse
{
    public required List<CreateOrderItemsResponse> OrderItems { get; set; }
    public required decimal TotalPrice { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}