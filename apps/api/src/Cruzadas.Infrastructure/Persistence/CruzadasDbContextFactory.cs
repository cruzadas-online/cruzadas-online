using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cruzadas.Infrastructure.Persistence;

public class CruzadasDbContextFactory : IDesignTimeDbContextFactory<CruzadasDbContext>
{
    public CruzadasDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CruzadasDbContext>();
        
        // Design-time connection string for migrations generation
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=55432;Database=cruzadas_db;Username=cruzadas_user;Password=cruzadas_secret";

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(CruzadasDbContext).Assembly.FullName);
        });

        return new CruzadasDbContext(optionsBuilder.Options);
    }
}
