using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;

namespace OrderService.Application.Interfaces;

public interface IOrderApplicationService
{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<GetOrderStatusResponse?> GetOrderStatusAsync(Guid orderId);

    Task CancelOrderAsync(Guid orderId);
    Task CompleteOrderAsync(Guid orderId);
}