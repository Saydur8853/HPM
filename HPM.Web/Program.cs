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
        options.DetailedErrors = true;
    });
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 32 * 1024 * 1024; // 32 MB for image uploads and camera frames
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
});
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 32 * 1024 * 1024; // 32 MB for Blazor ComponentHub
    options.EnableDetailedErrors = true;
});
builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<HospitalMasterCacheService>();
builder.Services.AddSingleton<UserAdministrationService>();
builder.Services.AddSingleton<HospitalSetupService>();
builder.Services.AddScoped<I18nService>();
builder.Services.AddScoped<PatientSelectionState>();
builder.Services.AddSingleton<IBarcodeQrService, BarcodeQrService>();

var connectionString = builder.Configuration.GetConnectionString("HpmDatabase")
    ?? throw new InvalidOperationException("Connection string 'HpmDatabase' was not found.");

builder.Services.AddDbContextFactory<HpmDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.CommandTimeout(30).EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null)));

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
        if (app.Environment.IsDevelopment())
            await HrmDemoDataSeeder.SeedAsync(db);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        if (!await roleManager.RoleExistsAsync(RoleNames.Authorizer))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(RoleNames.Authorizer));
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join("; ", roleResult.Errors.Select(x => x.Description)));
        }

        const string patientColSql = """
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Registration]') AND name = 'Weight')
            BEGIN
                ALTER TABLE [Registration] ADD [Weight] nvarchar(20) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Registration]') AND name = 'Photo_Data')
            BEGIN
                ALTER TABLE [Registration] ADD [Photo_Data] nvarchar(max) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctor_Appointment]') AND name = 'Weight')
            BEGIN
                ALTER TABLE [Doctor_Appointment] ADD [Weight] nvarchar(20) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctor_Appointment]') AND name = 'Sex')
            BEGIN
                ALTER TABLE [Doctor_Appointment] ADD [Sex] nvarchar(50) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Consultation_Time')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [Consultation_Time] nvarchar(100) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Off_Day')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [Off_Day] nvarchar(100) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'Consultation_Fee')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [Consultation_Fee] numeric(18,2) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Doctors_Profile]') AND name = 'OPD_Consultation_Fee')
            BEGIN
                ALTER TABLE [Doctors_Profile] ADD [OPD_Consultation_Fee] numeric(18,2) NULL;
            END
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Service]') AND name = 'Delivery_Hours')
            BEGIN
                ALTER TABLE [Service] ADD [Delivery_Hours] int NULL;
            END

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Registration_Contact' AND object_id = OBJECT_ID(N'[Registration]'))
                CREATE INDEX [IX_Registration_Contact] ON [Registration] ([Contact]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Registration_Name' AND object_id = OBJECT_ID(N'[Registration]'))
                CREATE INDEX [IX_Registration_Name] ON [Registration] ([Name]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Registration_Trans_Date' AND object_id = OBJECT_ID(N'[Registration]'))
                CREATE INDEX [IX_Registration_Trans_Date] ON [Registration] ([Trans_Date]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctor_Appointment_Appt_Date_Status' AND object_id = OBJECT_ID(N'[Doctor_Appointment]'))
                CREATE INDEX [IX_Doctor_Appointment_Appt_Date_Status] ON [Doctor_Appointment] ([Appt_Date], [Status]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctor_Appointment_Patient_ID' AND object_id = OBJECT_ID(N'[Doctor_Appointment]'))
                CREATE INDEX [IX_Doctor_Appointment_Patient_ID] ON [Doctor_Appointment] ([Patient_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctor_Appointment_Contact_No' AND object_id = OBJECT_ID(N'[Doctor_Appointment]'))
                CREATE INDEX [IX_Doctor_Appointment_Contact_No] ON [Doctor_Appointment] ([Contact_No]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OPD_Ticket_Trans_Date' AND object_id = OBJECT_ID(N'[OPD_Ticket]'))
                CREATE INDEX [IX_OPD_Ticket_Trans_Date] ON [OPD_Ticket] ([Trans_Date]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OPD_Ticket_Reg_No' AND object_id = OBJECT_ID(N'[OPD_Ticket]'))
                CREATE INDEX [IX_OPD_Ticket_Reg_No] ON [OPD_Ticket] ([Reg_No]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OPD_Ticket_Reg_No_Doctor_ID_Trans_Date' AND object_id = OBJECT_ID(N'[OPD_Ticket]'))
                CREATE INDEX [IX_OPD_Ticket_Reg_No_Doctor_ID_Trans_Date] ON [OPD_Ticket] ([Reg_No], [Doctor_ID], [Trans_Date]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Module_ID_Patient_Type_ID' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Module_ID_Patient_Type_ID] ON [Cash_Memo_DInvestigation] ([Module_ID], [Patient_Type], [ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Module_ID_Trans_Date' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Module_ID_Trans_Date] ON [Cash_Memo_DInvestigation] ([Module_ID], [Trans_Date]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Patient_ID' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Patient_ID] ON [Cash_Memo_DInvestigation] ([Patient_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_DInvestigation_Reg_No' AND object_id = OBJECT_ID(N'[Cash_Memo_DInvestigation]'))
                CREATE INDEX [IX_Cash_Memo_DInvestigation_Reg_No] ON [Cash_Memo_DInvestigation] ([Reg_No]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_OT_Trans_Date' AND object_id = OBJECT_ID(N'[Cash_Memo_OT]'))
                CREATE INDEX [IX_Cash_Memo_OT_Trans_Date] ON [Cash_Memo_OT] ([Trans_Date]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_OT_Patient_ID' AND object_id = OBJECT_ID(N'[Cash_Memo_OT]'))
                CREATE INDEX [IX_Cash_Memo_OT_Patient_ID] ON [Cash_Memo_OT] ([Patient_ID]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cash_Memo_OT_Reg_No' AND object_id = OBJECT_ID(N'[Cash_Memo_OT]'))
                CREATE INDEX [IX_Cash_Memo_OT_Reg_No] ON [Cash_Memo_OT] ([Reg_No]);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Service_Category_ID_Name' AND object_id = OBJECT_ID(N'[Service]'))
                CREATE INDEX [IX_Service_Category_ID_Name] ON [Service] ([Category_ID], [Name]);

            IF OBJECT_ID(N'[Patient_Operation]', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Patient_Operation_Operation_Date' AND object_id = OBJECT_ID(N'[Patient_Operation]'))
                    CREATE INDEX [IX_Patient_Operation_Operation_Date] ON [Patient_Operation] ([Operation_Date]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Patient_Operation_Patient_ID' AND object_id = OBJECT_ID(N'[Patient_Operation]'))
                    CREATE INDEX [IX_Patient_Operation_Patient_ID] ON [Patient_Operation] ([Patient_ID]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Patient_Operation_Reg_No' AND object_id = OBJECT_ID(N'[Patient_Operation]'))
                    CREATE INDEX [IX_Patient_Operation_Reg_No] ON [Patient_Operation] ([Reg_No]);
            END

            IF OBJECT_ID(N'[Operation_Note]', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Operation_Note_Operation_Date' AND object_id = OBJECT_ID(N'[Operation_Note]'))
                    CREATE INDEX [IX_Operation_Note_Operation_Date] ON [Operation_Note] ([Operation_Date]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Operation_Note_Patient_ID' AND object_id = OBJECT_ID(N'[Operation_Note]'))
                    CREATE INDEX [IX_Operation_Note_Patient_ID] ON [Operation_Note] ([Patient_ID]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Operation_Note_Reg_No' AND object_id = OBJECT_ID(N'[Operation_Note]'))
                    CREATE INDEX [IX_Operation_Note_Reg_No] ON [Operation_Note] ([Reg_No]);
            END

            IF OBJECT_ID(N'[Medical_Certificate]', N'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Medical_Certificate_Certificate_Type_Issue_Date' AND object_id = OBJECT_ID(N'[Medical_Certificate]'))
                    CREATE INDEX [IX_Medical_Certificate_Certificate_Type_Issue_Date] ON [Medical_Certificate] ([Certificate_Type], [Issue_Date]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Medical_Certificate_Patient_ID' AND object_id = OBJECT_ID(N'[Medical_Certificate]'))
                    CREATE INDEX [IX_Medical_Certificate_Patient_ID] ON [Medical_Certificate] ([Patient_ID]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Medical_Certificate_Reg_No' AND object_id = OBJECT_ID(N'[Medical_Certificate]'))
                    CREATE INDEX [IX_Medical_Certificate_Reg_No] ON [Medical_Certificate] ([Reg_No]);
            END
        """;
        await db.Database.ExecuteSqlRawAsync(patientColSql);

        var branding = app.Services.GetRequiredService<HospitalSetupService>();
        await branding.LoadAsync();
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Database migration or branding startup check skipped: {Message}", ex.Message);
    }
}

app.MapGet("/favicon.ico", (HospitalSetupService branding) =>
{
    var (bytes, contentType) = branding.GetFaviconBytes();
    return Results.File(bytes, contentType);
}).AllowAnonymous();

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

if (app.Environment.IsDevelopment())
{
    app.MapGet("/dev-login", async (SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) =>
    {
        var user = await userManager.FindByNameAsync("Saydur") ?? await userManager.FindByNameAsync("admin");
        if (user is not null)
        {
            await signInManager.SignInAsync(user, isPersistent: true);
            return Results.Redirect("/masters/doctors");
        }
        return Results.Redirect("/login");
    }).AllowAnonymous();
}

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
