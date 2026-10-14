using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Security;

public sealed class HospitalSetupService(
    IDbContextFactory<HpmDbContext> dbFactory,
    IServiceScopeFactory scopeFactory)
{
    private readonly SemaphoreSlim _loadLock = new(1, 1);

    public event Action? OnSettingsChanged;

    public string HospitalName { get; private set; } = "Hospital";
    public string Tagline { get; private set; } = "Excellence in Clinical Care & Diagnostic Medicine";
    public string Address { get; private set; } = "Central Hospital Road, Dhaka";
    public string CityDistrict { get; private set; } = "Dhaka, Bangladesh";
    public string Phone { get; private set; } = "01700-000000";
    public string Hotline { get; private set; } = "01700-000000";
    public string EmergencyContact { get; private set; } = "10678";
    public string Email { get; private set; } = "info@hospital.com";
    public string Website { get; private set; } = "";
    public string LicenseNo { get; private set; } = "";
    public string LogoData { get; private set; } = "";
    public string LabHeaderDept { get; private set; } = "Department of Pathology, Diagnostic & Laboratory Medicine";
    public string ReceiptHeaderNote { get; private set; } = "";
    public string ReceiptFooterNote { get; private set; } = "Computer generated clinical document. Valid without physical signature.";
    public bool IsSetupComplete { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    private static bool _schemaEnsured = true;
    private static readonly SemaphoreSlim _schemaLock = new(1, 1);

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await _loadLock.WaitAsync(cancellationToken);
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
            await EnsureSchemaAsync(db, cancellationToken);
            var profile = await db.HospitalProfiles.AsNoTracking()
                .SingleOrDefaultAsync(x => x.ID == 1, cancellationToken);
            ApplyProfile(profile);
        }
        finally
        {
            _loadLock.Release();
        }
    }

    private static async Task EnsureSchemaAsync(HpmDbContext db, CancellationToken ct)
    {
        if (_schemaEnsured) return;
        await _schemaLock.WaitAsync(ct);
        try
        {
            if (_schemaEnsured) return;
            const string sql = """
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Hospital_Profile]') AND name = 'Tagline')
                BEGIN
                    ALTER TABLE [Hospital_Profile] ADD [Tagline] nvarchar(200) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Address] nvarchar(255) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [City_District] nvarchar(100) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Phone] nvarchar(100) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Hotline] nvarchar(100) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Emergency_Contact] nvarchar(100) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Email] nvarchar(100) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Website] nvarchar(150) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [License_No] nvarchar(100) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Logo_Data] nvarchar(max) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Lab_Header_Dept] nvarchar(200) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Receipt_Header_Note] nvarchar(500) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Receipt_Footer_Note] nvarchar(500) NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Updated_At] datetime2 NULL;
                    ALTER TABLE [Hospital_Profile] ADD [Updated_By] nvarchar(100) NULL;
                END
                """;
            await db.Database.ExecuteSqlRawAsync(sql, ct);
            _schemaEnsured = true;
        }
        catch
        {
            // Ignore if already applied or non-sqlserver provider
        }
        finally
        {
            _schemaLock.Release();
        }
    }

    private void ApplyProfile(HospitalProfile? profile)
    {
        HospitalName = string.IsNullOrWhiteSpace(profile?.HospitalName) ? "Hospital" : profile.HospitalName;
        Tagline = string.IsNullOrWhiteSpace(profile?.Tagline) ? "Excellence in Clinical Care & Diagnostic Medicine" : profile.Tagline;
        Address = string.IsNullOrWhiteSpace(profile?.Address) ? "Central Hospital Road, Dhaka" : profile.Address;
        CityDistrict = string.IsNullOrWhiteSpace(profile?.CityDistrict) ? "Dhaka, Bangladesh" : profile.CityDistrict;
        Phone = string.IsNullOrWhiteSpace(profile?.Phone) ? "01700-000000" : profile.Phone;
        Hotline = string.IsNullOrWhiteSpace(profile?.Hotline) ? "01700-000000" : profile.Hotline;
        EmergencyContact = string.IsNullOrWhiteSpace(profile?.EmergencyContact) ? "10678" : profile.EmergencyContact;
        Email = string.IsNullOrWhiteSpace(profile?.Email) ? "info@hospital.com" : profile.Email;
        Website = profile?.Website ?? "";
        LicenseNo = profile?.LicenseNo ?? "";
        LogoData = profile?.LogoData ?? "";
        LabHeaderDept = string.IsNullOrWhiteSpace(profile?.LabHeaderDept) ? "Department of Pathology, Diagnostic & Laboratory Medicine" : profile.LabHeaderDept;
        ReceiptHeaderNote = profile?.ReceiptHeaderNote ?? "";
        ReceiptFooterNote = string.IsNullOrWhiteSpace(profile?.ReceiptFooterNote) ? "Computer generated clinical document. Valid without physical signature." : profile.ReceiptFooterNote;
        IsSetupComplete = profile?.IsSetupComplete == true;
        UpdatedAt = profile?.UpdatedAt ?? profile?.SetupCompletedAt;
        UpdatedBy = profile?.UpdatedBy ?? profile?.SetupBy;
    }

    public async Task<HospitalProfile> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        await EnsureSchemaAsync(db, cancellationToken);
        var profile = await db.HospitalProfiles.AsNoTracking()
            .SingleOrDefaultAsync(x => x.ID == 1, cancellationToken);

        if (profile is null)
        {
            profile = new HospitalProfile
            {
                ID = 1,
                HospitalName = HospitalName,
                Tagline = Tagline,
                Address = Address,
                CityDistrict = CityDistrict,
                Phone = Phone,
                Hotline = Hotline,
                EmergencyContact = EmergencyContact,
                Email = Email,
                Website = Website,
                LicenseNo = LicenseNo,
                LogoData = LogoData,
                LabHeaderDept = LabHeaderDept,
                ReceiptHeaderNote = ReceiptHeaderNote,
                ReceiptFooterNote = ReceiptFooterNote,
                IsSetupComplete = IsSetupComplete
            };
        }

        return profile;
    }

    public async Task<HospitalSetupResult> UpdateSettingsAsync(
        HospitalProfile input,
        string updatedBy,
        CancellationToken cancellationToken = default)
    {
        if (input == null)
            return HospitalSetupResult.Fail("Hospital profile data is required.");

        var name = (input.HospitalName ?? "").Trim();
        if (name.Length is < 2 or > 150)
            return HospitalSetupResult.Fail("Hospital name must be between 2 and 150 characters.");

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpmDbContext>();
        await EnsureSchemaAsync(db, cancellationToken);
        HospitalSetupResult? result = null;
        try
        {
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await db.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                var profile = await db.HospitalProfiles.SingleOrDefaultAsync(x => x.ID == 1, cancellationToken);
                if (profile is null)
                {
                    profile = new HospitalProfile
                    {
                        ID = 1,
                        IsSetupComplete = true,
                        SetupCompletedAt = DateTime.UtcNow,
                        SetupBy = updatedBy
                    };
                    db.HospitalProfiles.Add(profile);
                }

                profile.HospitalName = name;
                profile.Tagline = input.Tagline?.Trim() ?? "";
                profile.Address = input.Address?.Trim() ?? "";
                profile.CityDistrict = input.CityDistrict?.Trim() ?? "";
                profile.Phone = input.Phone?.Trim() ?? "";
                profile.Hotline = input.Hotline?.Trim() ?? "";
                profile.EmergencyContact = input.EmergencyContact?.Trim() ?? "";
                profile.Email = input.Email?.Trim() ?? "";
                profile.Website = input.Website?.Trim() ?? "";
                profile.LicenseNo = input.LicenseNo?.Trim() ?? "";
                profile.LogoData = input.LogoData?.Trim() ?? "";
                profile.LabHeaderDept = input.LabHeaderDept?.Trim() ?? "Department of Pathology, Diagnostic & Laboratory Medicine";
                profile.ReceiptHeaderNote = input.ReceiptHeaderNote?.Trim() ?? "";
                profile.ReceiptFooterNote = input.ReceiptFooterNote?.Trim() ?? "Computer generated clinical document. Valid without physical signature.";
                profile.UpdatedAt = DateTime.UtcNow;
                profile.UpdatedBy = updatedBy;

                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                ApplyProfile(profile);
                OnSettingsChanged?.Invoke();
                result = HospitalSetupResult.Ok();
            });
            return result ?? HospitalSetupResult.Ok();
        }
        catch (Exception ex)
        {
            return HospitalSetupResult.Fail($"Failed to save hospital settings: {ex.GetBaseException().Message}");
        }
    }

    public async Task<HospitalSetupResult> CompleteAsync(
        string hospitalName,
        string userName,
        string password,
        string confirmPassword,
        CancellationToken cancellationToken = default)
    {
        hospitalName = hospitalName.Trim();
        userName = userName.Trim();
        if (hospitalName.Length is < 2 or > 150)
            return HospitalSetupResult.Fail("Hospital name must contain 2 to 150 characters.");
        if (string.IsNullOrWhiteSpace(userName))
            return HospitalSetupResult.Fail("Superadmin user ID is required.");
        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            return HospitalSetupResult.Fail("Password and confirmation do not match.");

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpmDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        HospitalSetupResult? result = null;
        try
        {
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await db.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable, cancellationToken);
                await SqlServerConcurrency.AcquireTransactionLockAsync(db, "HPM:FirstRunSetup", cancellationToken);
                var profile = await db.HospitalProfiles.SingleOrDefaultAsync(x => x.ID == 1, cancellationToken);
                if (profile != null && (profile.IsSetupComplete || await db.Users.AnyAsync(cancellationToken)))
                {
                    result = HospitalSetupResult.Fail("Hospital setup has already been completed.");
                    return;
                }

                foreach (var roleName in RoleNames.All)
                {
                    if (!await roles.RoleExistsAsync(roleName))
                    {
                        var roleResult = await roles.CreateAsync(new IdentityRole<int>(roleName));
                        if (!roleResult.Succeeded) { result = HospitalSetupResult.Fail(Errors(roleResult)); return; }
                    }
                }

                var admin = new AppUser
                {
                    UserName = userName,
                    DisplayName = "Super Administrator",
                    IsActive = true,
                    MustChangePassword = false,
                    CreatedAt = DateTime.UtcNow
                };
                var createResult = await users.CreateAsync(admin, password);
                if (!createResult.Succeeded) { result = HospitalSetupResult.Fail(Errors(createResult)); return; }
                var roleAssignment = await users.AddToRoleAsync(admin, RoleNames.Administrator);
                if (!roleAssignment.Succeeded) { result = HospitalSetupResult.Fail(Errors(roleAssignment)); return; }

                if (profile is null)
                {
                    profile = new HospitalProfile { ID = 1 };
                    db.HospitalProfiles.Add(profile);
                }

                profile.HospitalName = hospitalName;
                profile.IsSetupComplete = true;
                profile.SetupCompletedAt = DateTime.UtcNow;
                profile.SetupBy = userName;
                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                ApplyProfile(profile);
                OnSettingsChanged?.Invoke();
                result = HospitalSetupResult.Ok();
            });
            return result ?? HospitalSetupResult.Ok();
        }
        catch (Exception ex)
        {
            return HospitalSetupResult.Fail($"Setup could not be completed. {ex.GetBaseException().Message}");
        }
    }

    private static string Errors(IdentityResult result) =>
        string.Join(" ", result.Errors.Select(x => x.Description));
}

public sealed record HospitalSetupResult(bool Succeeded, string Error)
{
    public static HospitalSetupResult Ok() => new(true, string.Empty);
    public static HospitalSetupResult Fail(string error) => new(false, error);
}

