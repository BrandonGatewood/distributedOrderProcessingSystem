namespace OrderService.Application.DTOs.Responses;

public class GetOrderStatusResponse
{
    public required Guid OrderId { get; set; }
    public required string Status { get; set; }
}