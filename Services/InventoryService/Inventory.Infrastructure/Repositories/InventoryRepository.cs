using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class InventoryRepository(InventoryDbContext context) : IInventoryRepository
{
    private readonly InventoryDbContext _context = context;

    public async Task<InventoryItem?> GetByProductIdAsync(Guid productId)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task UpdateAsync(InventoryItem inventoryItem)
    {
        _context.InventoryItems.Update(inventoryItem);
        await _context.SaveChangesAsync();
    }
}