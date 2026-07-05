using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services;

public class OrderApplicationService : IOrderApplicationService 
{
    public CreateOrderResponse CreateOrderAsync(CreateOrderRequest request)
    {
        // create order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        // save order to db

        // send order to message broker

        // return response
        return new CreateOrderResponse
        {
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
        
    }
}
