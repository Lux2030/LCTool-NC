using LCTool_NC.Data;
using LCTool_NC.Models;
using LCTool_NC.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {

        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddScoped<NonConformitaService>();
builder.Services.AddScoped<CurrentUserService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Crea il database e applica le migrazioni (no-op se già aggiornato)
    db.Database.Migrate();

    // Inserisce le voci delle liste solo se le tabelle sono vuote
    if (!db.NonConformitaStati.Any())
    {
        db.NonConformitaStati.AddRange(
            new NonConformitaStati { Nome = "Aperta", FlagAttivo = true },
            new NonConformitaStati { Nome = "In lavorazione", FlagAttivo = true },
            new NonConformitaStati { Nome = "Chiusa", FlagAttivo = true });
    }

    if (!db.NonConformitaRilevataDa.Any())
    {
        db.NonConformitaRilevataDa.AddRange(
            new NonConformitaRilevataDa { Valore = "Internamente", Ordine = 1, FlagAttivo = true },
            new NonConformitaRilevataDa { Valore = "Ente di Certificazione", Ordine = 2, FlagAttivo = true },
            new NonConformitaRilevataDa { Valore = "Corporate", Ordine = 3, FlagAttivo = true },
            new NonConformitaRilevataDa { Valore = "Audit Cliente", Ordine = 4, FlagAttivo = true },
            new NonConformitaRilevataDa { Valore = "Audit Interno", Ordine = 5, FlagAttivo = true });
    }

    if (!db.NonConformitaPartiInteressate.Any())
    {
        db.NonConformitaPartiInteressate.AddRange(
            new NonConformitaPartiInteressate { Valore = "Cliente", Ordine = 1, FlagAttivo = true },
            new NonConformitaPartiInteressate { Valore = "Fornitore", Ordine = 2, FlagAttivo = true },
            new NonConformitaPartiInteressate { Valore = "Dipendente", Ordine = 3, FlagAttivo = true },
            new NonConformitaPartiInteressate { Valore = "Ente di Certificazione", Ordine = 4, FlagAttivo = true });
    }

    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
