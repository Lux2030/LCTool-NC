using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LCTool_NC.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=LCToolDb;Trusted_Connection=True;TrustServerCertificate=True;";

        optionsBuilder
            .UseSqlServer(connectionString)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        return new AppDbContext(optionsBuilder.Options);
    }
}
