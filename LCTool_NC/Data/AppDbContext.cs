using LCTool_NC.Models;
using Microsoft.EntityFrameworkCore;

namespace LCTool_NC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NonConformita> NonConformita { get; set; }
    public DbSet<NonConformitaRilevataDa> NonConformitaRilevataDa { get; set; }
    public DbSet<NonConformitaStati> NonConformitaStati { get; set; }
    public DbSet<NonConformitaPartiInteressate> NonConformitaPartiInteressate { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NonConformita>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Numero).IsUnique();
            entity.Property(e => e.Stato).HasDefaultValue("Aperta");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });
    }
}
