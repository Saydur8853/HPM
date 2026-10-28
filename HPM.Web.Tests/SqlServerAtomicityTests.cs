using HPM.Web.Data;
using HPM.Web.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HPM.Web.Tests;

public sealed class SqlServerAtomicityTests
{
    [Fact]
    public async Task Fresh_migration_and_user_audit_are_atomic()
    {
        var connectionString = TestConnectionString();
        if (connectionString is null) return;

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddSingleton<AuthenticationStateProvider, WorkflowAndModelTests.TestAuthenticationStateProvider>();
        services.AddDbContextFactory<HpmDbContext>(options => options.UseSqlServer(
            connectionString,
            sql => sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(1), null)), ServiceLifetime.Scoped);
        services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<HpmDbContext>()
            .AddDefaultTokenProviders();
        services.AddSingleton<UserAdministrationService>();
        services.AddSingleton<HospitalSetupService>();

        await using var provider = services.BuildServiceProvider();
        await using var setupScope = provider.CreateAsyncScope();
        var setupDb = setupScope.ServiceProvider.GetRequiredService<HpmDbContext>();
        await setupDb.Database.EnsureDeletedAsync();
        try
        {
            await setupDb.Database.MigrateAsync();
            var hospitalSetup = provider.GetRequiredService<HospitalSetupService>();
            var setup = await hospitalSetup.CompleteAsync(
                "Automated Test Hospital",
                "superadmin",
                "Strong@Test123",
                "Strong@Test123");
            Assert.True(setup.Succeeded, setup.Error);

            await using (var verifySetupScope = provider.CreateAsyncScope())
            {
                var verifySetupDb = verifySetupScope.ServiceProvider.GetRequiredService<HpmDbContext>();
                Assert.True(await verifySetupDb.HospitalProfiles.AnyAsync(x => x.ID == 1 && x.IsSetupComplete));
                Assert.True(await verifySetupDb.Users.AnyAsync(x => x.UserName == "superadmin"));
            }

            var administration = provider.GetRequiredService<UserAdministrationService>();
            var created = await administration.CreateAsync(
                new UserAdminCommand(0, "Test User", "atomic-user", "atomic@example.local", RoleNames.LabTechnician, "Strong@Test123", true),
                null, "integration-test");
            Assert.True(created.Succeeded, created.Error);

            await using (var verifyScope = provider.CreateAsyncScope())
            {
                var verifyDb = verifyScope.ServiceProvider.GetRequiredService<HpmDbContext>();
                Assert.True(await verifyDb.Users.AnyAsync(x => x.UserName == "atomic-user" && x.MustChangePassword));
                Assert.True(await verifyDb.AuditLogs.AnyAsync(x => x.EntityName == "AppUser" && x.EntityID == created.UserId!.Value.ToString()));
            }

            await setupDb.Database.ExecuteSqlRawAsync("""
                CREATE OR ALTER TRIGGER [dbo].[TR_AuditLog_AtomicityTest]
                ON [dbo].[Audit_Log]
                INSTEAD OF INSERT
                AS THROW 51000, 'Intentional audit failure for atomicity test', 1;
                """);

            var rejected = await administration.CreateAsync(
                new UserAdminCommand(0, "Rollback User", "rollback-user", null, RoleNames.Reception, "Strong@Test123", true),
                null, "integration-test");
            Assert.False(rejected.Succeeded);

            await using (var verifyScope = provider.CreateAsyncScope())
            {
                var verifyDb = verifyScope.ServiceProvider.GetRequiredService<HpmDbContext>();
                Assert.False(await verifyDb.Users.AnyAsync(x => x.UserName == "rollback-user"));
            }
        }
        finally
        {
            await setupDb.Database.EnsureDeletedAsync();
        }
    }

    private static string? TestConnectionString()
    {
        var raw = Environment.GetEnvironmentVariable("HPM_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var builder = new SqlConnectionStringBuilder(raw);
        if (!builder.InitialCatalog.EndsWith("_AutomatedTests", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("HPM_TEST_CONNECTION must target a database ending in _AutomatedTests.");
        return builder.ConnectionString;
    }
}
