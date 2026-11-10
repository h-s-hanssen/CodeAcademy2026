using Kaffebar.Orm.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kaffebar.Orm;

// Oppgave 2A: Include og projeksjon.
public partial class KaffebarRepository
{
    /// <summary>Alle bestillinger med linjer, bønner og brennerier, i én spørring.</summary>
    public Task<List<Order>> GetAllOrdersWithDetailsAsync()
    {
        // TODO 2A: som GetAllOrdersAsync, men med hele grafen i én og samme spørring.
        //throw new NotImplementedException("Oppgave 2A");
        var order = db.Orders
            .Include(o => o.Lines)
                .ThenInclude(l => l.Bean)
                    .ThenInclude(b => b.Roastery);
        return order.ToListAsync();

    }

    /// <summary>Kvitteringene som DTO-er, uten å laste entiteter.</summary>
    public Task<List<ReceiptDto>> GetReceiptsAsync()
    {
        // TODO 2A, del 2: bygg ReceiptDto direkte i spørringen med Select. Ingen Include.
        //throw new NotImplementedException("Oppgave 2A, del 2");
        var receipts = db.Orders
                  .Select(o => new ReceiptDto(
                      o.CustomerName,
                      o.Status,
                      o.TotalPrice,
                      o.Lines.Select(l => new ReceiptLineDto(
                          l.Quantity,
                          l.DrinkName,
                          l.Size,
                          l.Bean.Name,
                          l.Bean.Roastery.Name
                      )).ToList()
                  ));
        return receipts.ToListAsync();
    }
}
