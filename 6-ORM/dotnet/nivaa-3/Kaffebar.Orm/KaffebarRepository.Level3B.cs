using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Kaffebar.Orm;

// Oppgave 3B: Entiteter ut av datalaget. En kollega har skrevet dette, og testene er røde.
public partial class KaffebarRepository
{
    /// <summary>
    /// Bestillingen som JSON til kassa-appen, eller <c>null</c> hvis den ikke finnes.
    /// Kassa-appen forventer dette formatet:
    /// <code>
    /// {
    ///   "id": "0de50001-0000-4000-8000-000000000004",
    ///   "customerName": "Ingrid",
    ///   "status": "Pending",
    ///   "totalPrice": 226.00,
    ///   "lines": [
    ///     { "bean": "Yirgacheffe", "roastery": "Fuglen", "quantity": 3, "unitPrice": 59.00 }
    ///   ],
    ///   "statusHistory": [ "Pending" ]
    /// }
    /// </code>
    /// </summary>
    public async Task<string?> GetOrderJsonAsync(Guid orderId)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .ThenInclude(l => l.Bean)
            .ThenInclude(b => b.Roastery)
            .Include(o => o.StatusHistory)
            .SingleOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            return null;

        var dto = new
        {
            id = order.Id,
            customerName = order.CustomerName,  
            status = order.Status.ToString(),
            totalPrice = order.TotalPrice,
            lines = order.Lines.Select(l => new
            {
                bean = l.Bean.Name,
                roastery = l.Bean.Roastery.Name,
                quantity = l.Quantity,
                unitPrice = l.UnitPrice
            }),
            statusHistory = order.StatusHistory.Select(h => h.Status.ToString())
        };

        return JsonSerializer.Serialize(dto, JsonSerializerOptions.Web);
    }
}
