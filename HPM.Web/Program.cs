using HPM.Web.Components;
using HPM.Web.Data;
using HPM.Web.Localization;
using HPM.Web.Security;
using HPM.Web.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, logger) => logger
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/primehpm-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(15);
        options.DisconnectedCircuitMaxRetained = 200;
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
    });
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<UserAdministrationService>();
builder.Services.AddSingleton<HospitalSetupService>();
builder.Services.AddScoped<I18nService>();
builder.Services.AddScoped<PatientSelectionState>();
builder.Services.AddSingleton<IBarcodeQrService, BarcodeQrService>();

var connectionString = builder.Configuration.GetConnectionString("HpmDatabase")
    ?? throw new InvalidOperationException("Connection string 'HpmDatabase' was not found.");

builder.Services.AddDbContextFactory<HpmDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.CommandTimeout(180).EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null)));

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<HpmDbContext>()
    .AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(15);
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PasswordChangeAllowed", policy => policy.RequireAuthenticatedUser());
    options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireAssertion(context => !context.User.HasClaim(
            AppUserClaimsPrincipalFactory.MustChangePasswordClaim,
            bool.TrueString))
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

await using (var scope = app.Services.CreateAsyncScope())
{
    try
    {
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HpmDbContext>>();
        await using var db = await dbFactory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Database migration check skipped due to startup delay: {Message}", ex.Message);
    }
}

app.MapGet("/health", async (IDbContextFactory<HpmDbContext> dbFactory) =>
{
    try
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var databaseReady = await db.Database.CanConnectAsync();
        return Results.Json(new { status = databaseReady ? "Healthy" : "Unhealthy", database = databaseReady, utc = DateTime.UtcNow }, statusCode: databaseReady ? 200 : 503);
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "Unhealthy", database = false, error = ex.GetType().Name, utc = DateTime.UtcNow }, statusCode: 503);
    }
}).AllowAnonymous();

app.MapPost("/account/login", async (HttpContext http, IAntiforgery antiforgery, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) =>
{
    await antiforgery.ValidateRequestAsync(http);
    var form = await http.Request.ReadFormAsync();
    var userName = form["UserName"].ToString().Trim();
    var password = form["Password"].ToString();
    var rememberMe = string.Equals(form["RememberMe"], "true", StringComparison.OrdinalIgnoreCase);
    var returnUrl = SafeReturnUrl(form["ReturnUrl"].ToString());
    var user = await userManager.FindByNameAsync(userName);

    if (user is not null && user.IsActive)
    {
        var result = await signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
            return Results.LocalRedirect(user.MustChangePassword ? "/account/change-password" : returnUrl);
    }

    return Results.LocalRedirect($"/login?error=1&returnUrl={Uri.EscapeDataString(returnUrl)}");
}).AllowAnonymous();

app.MapPost("/account/logout", async (HttpContext http, IAntiforgery antiforgery, SignInManager<AppUser> signInManager) =>
{
    await antiforgery.ValidateRequestAsync(http);
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/login");
}).RequireAuthorization("PasswordChangeAllowed");

app.MapPost("/account/change-password-submit", async (HttpContext http, IAntiforgery antiforgery, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IDbContextFactory<HpmDbContext> dbFactory) =>
{
    await antiforgery.ValidateRequestAsync(http);
    var form = await http.Request.ReadFormAsync();
    var currentPassword = form["CurrentPassword"].ToString();
    var newPassword = form["NewPassword"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();
    if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
        return Results.LocalRedirect("/account/change-password?error=mismatch");

    var user = await userManager.GetUserAsync(http.User);
    if (user is null) return Results.LocalRedirect("/login");
    var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    if (!result.Succeeded) return Results.LocalRedirect("/account/change-password?error=invalid");

    user.MustChangePassword = false;
    var updateResult = await userManager.UpdateAsync(user);
    if (!updateResult.Succeeded) return Results.LocalRedirect("/account/change-password?error=invalid");

    await signInManager.RefreshSignInAsync(user);
    await using var db = await dbFactory.CreateDbContextAsync();
    db.AuditLogs.Add(new HPM.Web.Data.Entities.AuditLog
    {
        UserID = user.Id,
        UserName = user.UserName ?? "unknown",
        Action = "Update",
        EntityName = "AppUser",
        EntityID = user.Id.ToString(),
        ChangedFields = "Password changed by account owner",
        EventTime = DateTime.UtcNow
    });
    await db.SaveChangesAsync();
    return Results.LocalRedirect("/account/change-password?success=1");
}).RequireAuthorization("PasswordChangeAllowed");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static string SafeReturnUrl(string? returnUrl)
{
    if (string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith('/') || returnUrl.StartsWith("//")) return "/";
    return returnUrl;
}
