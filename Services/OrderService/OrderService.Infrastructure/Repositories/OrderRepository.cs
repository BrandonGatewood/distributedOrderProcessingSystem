using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrderRepository
{
    private readonly OrderDbContext _context = context;

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus status)
    {
       var rowsAffected = await _context.Orders
        .Where(o => o.Id == orderId && o.Status == OrderStatus.Pending)
        .ExecuteUpdateAsync(setters =>
            setters.SetProperty(
                o => o.Status,
                status));

        return rowsAffected > 0; 
    }
}