using Microsoft.EntityFrameworkCore;

namespace Kaffebar.Orm;

// Oppgave 4B: Den komplekse lesemodellen.
public partial class KaffebarRepository
{
    /// <summary>Topp 3 bønner per brenneri per måned, målt i antall kopper, med omsetning.</summary>
    public Task<List<BeanSalesRow>> GetTopBeansPerRoasteryPerMonthAsync()
    {
        // TODO 4B: rå SQL med db.Database.SqlQuery<BeanSalesRow>($"...").
        //   Tabellene heter orders, order_lines, beans og roasteries (snake_case).
        //   Kolonnene i resultatet må hete det samme som egenskapene i BeanSalesRow,
        //   i snake_case: month, roastery, bean, cups_sold, revenue, rank.
        //   Rangering innen hver måned og hvert brenneri: flest kopper, så høyest omsetning,
        //   så bønnenavn. Sorter resultatet på brenneri, måned og rang.
        //   Måneden må være en date (starten av måneden i UTC) for å bli en DateOnly.
        //throw new NotImplementedException("Oppgave 4B");

        //return db.FromSqlRaw(@"
        //    SELECT
        //        DATE_TRUNC('month', o.ordered_at) AS month,
        //        r.name AS roastery,
        //        b.name AS bean,
        //        SUM(ol.quantity) AS cups_sold,
        //        SUM(ol.quantity * ol.price) AS revenue,
        //        RANK() OVER (PARTITION BY DATE_TRUNC('month', o.ordered_at), r.name ORDER BY SUM(ol.quantity) DESC, SUM(ol.quantity * ol.price) DESC, b.name ASC) AS rank
        //    FROM orders o
        //    JOIN order_lines ol ON o.id = ol.order_id
        //    JOIN beans b ON ol.bean_id = b.id
        //    JOIN roasteries r ON b.roastery_id = r.id
        //    GROUP BY DATE_TRUNC('month', o.ordered_at), r.name, b.name")
        //    .ToListAsync();
        //return 

        return Task.FromResult(db.Database.FromSqlRaw<BeanSalesRow>(@"
            SELECT
                DATE_TRUNC('month', o.ordered_at) AS month,
                r.name AS roastery,
                b.name AS bean,
                SUM(ol.quantity) AS cups_sold,
                SUM(ol.quantity * ol.price) AS revenue,
                RANK() OVER (PARTITION BY DATE_TRUNC('month', o.ordered_at), r.name ORDER BY SUM(ol.quantity) DESC, SUM(ol.quantity * ol.price) DESC, b.name ASC) AS rank
            FROM orders o
            JOIN order_lines ol ON o.id = ol.order_id
            JOIN beans b ON ol.bean_id = b.id
            JOIN roasteries r ON b.roastery_id = r.id
            GROUP BY DATE_TRUNC('month', o.ordered_at), r.name, b.name").ToList());
    }

    /// <summary>Bonus: den samme rapporten, kjørt rett på ADO.NET-tilkoblingen.</summary>
    public Task<List<BeanSalesRow>> GetTopBeansWithAdoNetAsync()
    {
        // TODO 4B, bonus: samme SQL via db.Database.GetDbConnection(). Åpne tilkoblingen,
        //   lag en DbCommand, og map hver rad til BeanSalesRow selv.
        throw new NotImplementedException("Oppgave 4B, bonus");
    }
}
