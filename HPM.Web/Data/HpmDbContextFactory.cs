using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Security.Claims;

namespace HPM.Web.Data;

public sealed class HpmDbContextFactory : IDesignTimeDbContextFactory<HpmDbContext>
{
    public HpmDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            basePath = Path.Combine(basePath, "HPM.Web");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("HpmDatabase")
            ?? throw new InvalidOperationException("Connection string 'HpmDatabase' was not found.");
        var options = new DbContextOptionsBuilder<HpmDbContext>()
            .UseSqlServer(connectionString, sql => sql.CommandTimeout(180))
            .Options;

        return new HpmDbContext(options);
    }
}
