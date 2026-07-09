using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Services;

public class OrderApplicationService(IOrderRepository orderRepository) : IOrderApplicationService 
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        Guid orderId = Guid.NewGuid();

        var orderItems = request.OrderItems.Select(i => new OrderItem
        {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
        }).ToList();

        // create order
        var order = new Order
        {
            Id = orderId,
            UserId = request.UserId,
            OrderItems = orderItems,
            TotalAmount = orderItems.Sum(i => i.LineTotal),
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // save order to db
        await _orderRepository.AddAsync(order);

        // send order to message broker

        // return response
        return new CreateOrderResponse
        {
            OrderItems = order.OrderItems.Select(i => new CreateOrderItemsResponse
            {
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList(),
            TotalPrice = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt
        };
    }
}
