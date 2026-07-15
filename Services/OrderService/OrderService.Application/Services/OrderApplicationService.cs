using Messaging.Contracts.Events;
using OrderService.Application.DTOs.Requests;
using OrderService.Application.DTOs.Responses;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

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
        await _eventPublisher.PublishAsync("order.created", message);

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
