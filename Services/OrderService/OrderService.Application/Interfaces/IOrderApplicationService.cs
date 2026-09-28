using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;

namespace OrderService.Application.Interfaces;

public interface IOrderApplicationService
{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<GetOrderStatusResponse?> GetOrderStatusAsync(Guid orderId);
}