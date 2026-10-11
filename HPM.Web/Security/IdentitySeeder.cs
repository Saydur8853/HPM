using Microsoft.AspNetCore.Identity;

namespace HPM.Web.Security;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        foreach (var roleName in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
            }
        }

        var userName = configuration["InitialAdmin:UserName"];
        var password = configuration["InitialAdmin:Password"];
        if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(password)) return;
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("InitialAdmin requires both UserName and Password.");
        if (string.Equals(password, "Admin@12345", StringComparison.Ordinal))
            throw new InvalidOperationException("The insecure default InitialAdmin password is not allowed. Configure a unique password through environment variables.");

        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            user = new AppUser
            {
                UserName = userName,
                Email = configuration["InitialAdmin:Email"],
                DisplayName = configuration["InitialAdmin:DisplayName"] ?? "System Administrator",
                EmailConfirmed = true,
                IsActive = true,
                MustChangePassword = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }

        if (!await userManager.IsInRoleAsync(user, RoleNames.Administrator))
        {
            var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Administrator);
            if (!roleResult.Succeeded) throw new InvalidOperationException(string.Join("; ", roleResult.Errors.Select(x => x.Description)));
        }
    }
}
