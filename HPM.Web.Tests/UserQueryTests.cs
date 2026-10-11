using HPM.Web.Data;
using HPM.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace HPM.Web.Tests;

public sealed class UserQueryTests
{
    private readonly ITestOutputHelper _output;

    public UserQueryTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task List_Or_Ensure_Admin_User()
    {
        var connectionString = "Server=np:\\\\.\\pipe\\MSSQL$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30";
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddDbContextFactory<HpmDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);
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

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var db = scope.ServiceProvider.GetRequiredService<HpmDbContext>();

        var users = await db.Users.ToListAsync();
        _output.WriteLine($"Total users found: {users.Count}");
        foreach (var u in users)
        {
            _output.WriteLine($"User: {u.UserName} | Display: {u.DisplayName} | Active: {u.IsActive} | MustChangePwd: {u.MustChangePassword}");
        }

        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser != null)
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var res = await userManager.ResetPasswordAsync(adminUser, resetToken, "Admin@PrimeHpm2026!");
            _output.WriteLine($"Reset admin password result: {res.Succeeded}");
            adminUser.MustChangePassword = false;
            adminUser.IsActive = true;
            await userManager.UpdateAsync(adminUser);
            _output.WriteLine("Admin password set to Admin@PrimeHpm2026! and MustChangePassword set to false.");
        }
        else
        {
            _output.WriteLine("Admin user not found, creating admin...");
            var newAdmin = new AppUser
            {
                UserName = "admin",
                DisplayName = "System Administrator",
                Email = "admin@primehpm.local",
                EmailConfirmed = true,
                IsActive = true,
                MustChangePassword = false,
                CreatedAt = DateTime.UtcNow
            };
            var createRes = await userManager.CreateAsync(newAdmin, "Admin@PrimeHpm2026!");
            _output.WriteLine($"Created admin: {createRes.Succeeded}");
            if (!await roleManager.RoleExistsAsync(RoleNames.Administrator))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(RoleNames.Administrator));
            }
            await userManager.AddToRoleAsync(newAdmin, RoleNames.Administrator);
        }
    }
}
