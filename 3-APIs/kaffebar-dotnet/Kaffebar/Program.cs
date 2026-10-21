using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Registrer OpenAPI-tjenestene. Disse genererer spesifikasjonen
// automatisk basert på endepunktene og typene i prosjektet (code-first).
builder.Services.AddOpenApi();

var app = builder.Build();
var users = new[]
{
    new User(Guid.Parse("1e6d4562-9eaf-4ec3-8613-6f15afb92bd9"), "Ada Lovelace", true),
    new User(Guid.Parse("4563d1f1-2db5-4e80-b302-790b5d8c7e93"), "Alan Turing", false),
    new User(Guid.Parse("6b59c42b-4bf4-47b9-b28c-b2a3a95e91ef"), "Grace Hopper", true)
};

if (app.Environment.IsDevelopment())
{
    // Eksponerer den genererte spesifikasjonen på /openapi/v1.json
    app.MapOpenApi();

    // Scalar gir et moderne, interaktivt UI på /scalar/v1
    // for å utforske og teste API-et.
    app.MapScalarApiReference();
}

// --- Hello Coffee --------------------------------------------------------
// Et minimal API-endepunkt som returnerer en hardkodet kaffemeny.
// Dette er utgangspunktet for workshopen. Bygg videre herfra!
app.MapGet("/menu", () => new[]
{
    new Coffee(Guid.NewGuid(), "Kaffe Latte", 48.50m),
    new Coffee(Guid.NewGuid(), "Cappuccino", 45.00m),
    new Coffee(Guid.NewGuid(), "Espresso", 35.00m)
})
.WithName("GetMenu")
.WithSummary("Hent kaffemeny")
.WithDescription("Returnerer en liste over alle tilgjengelige kaffedrikker i kaffebaren.");

app.MapGet("/users/active", () => users.Where(user => user.Active))
    .WithName("GetActiveUsers")
    .WithSummary("Hent aktive brukere")
    .WithDescription("Returnerer alle brukere som er aktive.")
    .Produces<IEnumerable<User>>();

app.Run();

// DTO-er kan ligge i Program.cs når prosjektet er lite.
// Etter hvert er det ryddig å flytte dem til egne filer i en Models-mappe.
public record Coffee(Guid Id, string Name, decimal Price);

public record User(Guid Id, string Name, bool Active);

public partial class Program;
