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

        try
        {
            var profile = await ResilientTransaction.ExecuteScopedAsync(
                scopeFactory,
                async (_, db) =>
                {
                    await EnsureSchemaAsync(db, cancellationToken);
                    var savedProfile = await db.HospitalProfiles.SingleOrDefaultAsync(x => x.ID == 1, cancellationToken);
                    if (savedProfile is null)
                    {
                        savedProfile = new HospitalProfile
                        {
                            ID = 1,
                            IsSetupComplete = true,
                            SetupCompletedAt = DateTime.UtcNow,
                            SetupBy = updatedBy
                        };
                        db.HospitalProfiles.Add(savedProfile);
                    }

                    savedProfile.HospitalName = name;
                    savedProfile.Tagline = input.Tagline?.Trim() ?? "";
                    savedProfile.Address = input.Address?.Trim() ?? "";
                    savedProfile.CityDistrict = input.CityDistrict?.Trim() ?? "";
                    savedProfile.Phone = input.Phone?.Trim() ?? "";
                    savedProfile.Hotline = input.Hotline?.Trim() ?? "";
                    savedProfile.EmergencyContact = input.EmergencyContact?.Trim() ?? "";
                    savedProfile.Email = input.Email?.Trim() ?? "";
                    savedProfile.Website = input.Website?.Trim() ?? "";
                    savedProfile.LicenseNo = input.LicenseNo?.Trim() ?? "";
                    savedProfile.LogoData = input.LogoData?.Trim() ?? "";
                    savedProfile.LabHeaderDept = input.LabHeaderDept?.Trim() ?? "Department of Pathology, Diagnostic & Laboratory Medicine";
                    savedProfile.ReceiptHeaderNote = input.ReceiptHeaderNote?.Trim() ?? "";
                    savedProfile.ReceiptFooterNote = input.ReceiptFooterNote?.Trim() ?? "Computer generated clinical document. Valid without physical signature.";
                    savedProfile.UpdatedAt = DateTime.UtcNow;
                    savedProfile.UpdatedBy = updatedBy;

                    await db.SaveChangesAsync(cancellationToken);
                    return savedProfile;
                },
                cancellationToken: cancellationToken);

            ApplyProfile(profile);
            OnSettingsChanged?.Invoke();

            return HospitalSetupResult.Ok();
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

        try
        {
            var profile = await ResilientTransaction.ExecuteScopedAsync(
                scopeFactory,
                async (services, db) =>
                {
                    var users = services.GetRequiredService<UserManager<AppUser>>();
                    var roles = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                    await SqlServerConcurrency.AcquireTransactionLockAsync(db, "HPM:FirstRunSetup", cancellationToken);
                    var savedProfile = await db.HospitalProfiles.SingleOrDefaultAsync(x => x.ID == 1, cancellationToken);
                    if (savedProfile != null && (savedProfile.IsSetupComplete || await db.Users.AnyAsync(cancellationToken)))
                        throw new InvalidOperationException("Hospital setup has already been completed.");

                    foreach (var roleName in RoleNames.All)
                    {
                        if (!await roles.RoleExistsAsync(roleName))
                        {
                            var roleResult = await roles.CreateAsync(new IdentityRole<int>(roleName));
                            if (!roleResult.Succeeded) throw new InvalidOperationException(Errors(roleResult));
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
                    if (!createResult.Succeeded) throw new InvalidOperationException(Errors(createResult));
                    var roleAssignment = await users.AddToRoleAsync(admin, RoleNames.Administrator);
                    if (!roleAssignment.Succeeded) throw new InvalidOperationException(Errors(roleAssignment));

                    if (savedProfile is null)
                    {
                        savedProfile = new HospitalProfile { ID = 1 };
                        db.HospitalProfiles.Add(savedProfile);
                    }

                    savedProfile.HospitalName = hospitalName;
                    savedProfile.IsSetupComplete = true;
                    savedProfile.SetupCompletedAt = DateTime.UtcNow;
                    savedProfile.SetupBy = userName;
                    await db.SaveChangesAsync(cancellationToken);
                    return savedProfile;
                },
                System.Data.IsolationLevel.Serializable,
                cancellationToken);

            ApplyProfile(profile);
            OnSettingsChanged?.Invoke();
            return HospitalSetupResult.Ok();
        }
        catch (Exception ex)
        {
            return HospitalSetupResult.Fail($"Setup could not be completed. {ex.GetBaseException().Message}");
        }
    }

    private static string Errors(IdentityResult result) =>
        string.Join(" ", result.Errors.Select(x => x.Description));

    public string GetAvatarInitials()
    {
        var name = HospitalName?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Equals("Hospital", StringComparison.OrdinalIgnoreCase))
            return "H+";

        var words = name.Split([' ', '-', ',', '.', '&'], StringSplitOptions.RemoveEmptyEntries)
                        .Where(w => !w.Equals("and", StringComparison.OrdinalIgnoreCase) &&
                                    !w.Equals("of", StringComparison.OrdinalIgnoreCase) &&
                                    !w.Equals("the", StringComparison.OrdinalIgnoreCase))
                        .ToArray();

        if (words.Length == 0)
            return "H+";

        if (words.Length == 1)
        {
            var word = words[0];
            return word.Length <= 2 ? word.ToUpperInvariant() : word[..2].ToUpperInvariant();
        }

        return $"{char.ToUpperInvariant(words[0][0])}{char.ToUpperInvariant(words[1][0])}";
    }

    public string GetFaviconHref()
    {
        if (!string.IsNullOrWhiteSpace(LogoData))
        {
            return LogoData;
        }

        return BuildAvatarSvgDataUri(GetAvatarInitials());
    }

    public static string BuildAvatarSvgDataUri(string initials)
    {
        var svg = BuildAvatarSvg(initials);
        var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg));
        return $"data:image/svg+xml;base64,{base64}";
    }

    public static string BuildAvatarSvg(string initials)
    {
        var cleanInitials = System.Security.SecurityElement.Escape(string.IsNullOrWhiteSpace(initials) ? "H+" : initials);
        var fontSize = cleanInitials.Length > 2 ? "20" : "24";
        return $"""
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64" width="64" height="64">
              <defs>
                <linearGradient id="hpmFavGrad" x1="0%" y1="0%" x2="100%" y2="100%">
                  <stop offset="0%" stop-color="#0284c7" />
                  <stop offset="100%" stop-color="#0f172a" />
                </linearGradient>
              </defs>
              <rect width="64" height="64" rx="16" fill="url(#hpmFavGrad)" />
              <rect x="2" y="2" width="60" height="60" rx="14" fill="none" stroke="#38bdf8" stroke-width="2" stroke-opacity="0.5" />
              <circle cx="50" cy="14" r="5" fill="#10b981" />
              <text x="32" y="38" font-family="-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif" font-size="{fontSize}" font-weight="800" fill="#ffffff" text-anchor="middle" dominant-baseline="central">{cleanInitials}</text>
            </svg>
            """;
    }

    public (byte[] Bytes, string ContentType) GetFaviconBytes()
    {
        if (!string.IsNullOrWhiteSpace(LogoData) && LogoData.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var commaIdx = LogoData.IndexOf(',');
                if (commaIdx > 5)
                {
                    var header = LogoData[..commaIdx];
                    var base64 = LogoData[(commaIdx + 1)..];
                    var mime = "image/png";
                    var semiIdx = header.IndexOf(';');
                    if (semiIdx > 5)
                    {
                        mime = header[5..semiIdx];
                    }
                    var bytes = Convert.FromBase64String(base64);
                    return (bytes, mime);
                }
            }
            catch
            {
                // Fallback to SVG on parse failure
            }
        }

        var svg = BuildAvatarSvg(GetAvatarInitials());
        return (System.Text.Encoding.UTF8.GetBytes(svg), "image/svg+xml");
    }
}

public sealed record HospitalSetupResult(bool Succeeded, string Error)
{
    public static HospitalSetupResult Ok() => new(true, string.Empty);
    public static HospitalSetupResult Fail(string error) => new(false, error);
}

