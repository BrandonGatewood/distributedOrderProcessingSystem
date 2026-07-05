using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;

namespace OrderService.Application.Interfaces;

public interface IOrderApplicationService
{
    CreateOrderResponse CreateOrderAsync(CreateOrderRequest request);
}