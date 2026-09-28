using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class InventoryRepository(InventoryDbContext context) : IInventoryRepository
{
    private readonly InventoryDbContext _context = context;

    public async Task<bool> TryReserveOrderAsync(IEnumerable<InventoryItem> items, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        // Always lock products in the same order.
        var orderedItems = items
            .OrderBy(x => x.ProductId)
            .ToList();

        foreach (var item in orderedItems)
        {
            var inventory = await _context.InventoryItems
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM "InventoryItems"
                    WHERE "ProductId" = {item.ProductId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            // Product doesn't exist.
            if (inventory is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            // Not enough inventory.
            if (inventory.Quantity < item.Quantity)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            // Reserve the inventory.
            inventory.Quantity -= item.Quantity;
        }

        // Persist all inventory changes.
        await _context.SaveChangesAsync(cancellationToken);

        // Make all changes permanent.
        await transaction.CommitAsync(cancellationToken);

        return true;
    }
}