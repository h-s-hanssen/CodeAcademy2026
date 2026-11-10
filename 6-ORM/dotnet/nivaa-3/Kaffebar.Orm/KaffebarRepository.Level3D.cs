using Kaffebar.Orm.Domain;

namespace Kaffebar.Orm;

// Oppgave 3D (bonus): Den motsatte fellen. En kollega har skrevet dette, og testene er røde.
public partial class KaffebarRepository
{
    /// <summary>Lagrer en ordre kassa-appen hentet tidligere og nå sender inn igjen med ny status.</summary>
    public async Task SaveOrderAsync(Order order)
    {
        // Ordren kommer fra en annen (nå lukket) context, så den må knyttes til denne
        // contexten igjen før EF kan oppdage og lagre endringene.
        db.Attach(order);
        db.Entry(order).Property(o => o.Status).IsModified = true;
        await db.SaveChangesAsync();
    }
}
