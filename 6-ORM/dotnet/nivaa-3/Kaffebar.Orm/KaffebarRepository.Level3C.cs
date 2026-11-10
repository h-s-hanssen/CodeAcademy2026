using Kaffebar.Orm.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kaffebar.Orm;

// Oppgave 3C (bonus): Masseoppdatering.
public partial class KaffebarRepository
{
    /// <summary>Alt som har stått Ready siden før <paramref name="cutoff"/> er hentet.</summary>
    public async Task<int> PickUpStaleOrdersAsync(DateTimeOffset cutoff)
    {
        var readyOrders = db.Orders
            .Where(o => o.Status == OrderStatus.Ready && o.CreatedAt < cutoff);

        // Skriv historikken mens ordrene fortsatt står som Ready i databasen,
        // ellers matcher ikke predikatet lenger etter ExecuteUpdateAsync.
        await db.Database.ExecuteSqlAsync(
            $"""
             INSERT INTO status_changes (order_id, status, changed_at)
             SELECT id, 'PickedUp', {DateTimeOffset.UtcNow}
             FROM orders
             WHERE status = 'Ready' AND created_at < {cutoff}
             """);

        return await readyOrders.ExecuteUpdateAsync(o => o.SetProperty(o => o.Status, OrderStatus.PickedUp));
    }
}
