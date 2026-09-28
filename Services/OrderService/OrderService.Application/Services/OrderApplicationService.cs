using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using Shared.Contracts.Events;
using Shared.Messaging.Constants;
using Shared.Messaging.Interfaces;

namespace OrderService.Application.Services;

public class OrderApplicationService(IOrderRepository orderRepository, IEventPublisher eventPublisher) : IOrderApplicationService 
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        // Create a new order with the provided order items
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

        // create event message
        var message = new OrderCreatedEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Items = order.OrderItems.Select(i => new OrderItemEvent
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        // publish event 
        await _eventPublisher.PublishAsync(RabbitMqConstants.OrderExchange, RabbitMqConstants.OrderCreatedRoutingKey, message);

        return new CreateOrderResponse
        {
            Id = order.Id,
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

    public async Task<GetOrderStatusResponse?> GetOrderStatusAsync(Guid orderId)
{
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            return null;

        return new GetOrderStatusResponse
        {
            OrderId = order.Id,
            Status = order.Status.ToString()
        };
    }
}
