using System.Security.Claims;
using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Security;

public sealed class HospitalLicenseService(IDbContextFactory<HpmDbContext> dbFactory)
{
    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    private static volatile HashSet<string>? _cachedEnabledModules;

    public static event Action? OnLicenseChanged;

    public void InvalidateCache()
    {
        _cachedEnabledModules = null;
        OnLicenseChanged?.Invoke();
    }

    public async Task<HashSet<string>> EnabledModulesAsync(CancellationToken ct = default)
    {
        if (_cachedEnabledModules is { } cached)
            return cached;

        await CacheLock.WaitAsync(ct);
        try
        {
            if (_cachedEnabledModules is { } doubleChecked)
                return doubleChecked;

            await using var db = await dbFactory.CreateDbContextAsync(ct);
            var license = await db.HospitalLicenses.AsNoTracking().SingleOrDefaultAsync(x => x.ID == 1, ct);
            HashSet<string> result;
            if (license is null)
            {
                result = LicensedModules.All.Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase); // existing installations retain access
            }
            else if (!license.IsActivated)
            {
                result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                var enabled = await db.HospitalModuleLicenses.AsNoTracking().Where(x => x.IsEnabled).Select(x => x.ModuleCode).ToListAsync(ct);
                result = LicensedModules.WithDependencies(enabled);
            }

            _cachedEnabledModules = result;
            return result;
        }
        finally
        {
            CacheLock.Release();
        }
    }

    public bool TryCanOpen(ClaimsPrincipal user, Type pageType, out bool allowed)
    {
        if (TryResolveStaticPageAccess(user, pageType, out var module, out allowed))
            return true;

        if (_cachedEnabledModules is { } enabled)
        {
            allowed = EvaluateModuleAccess(pageType, module!, enabled);
            return true;
        }

        allowed = false;
        return false;
    }

    public async Task<bool> CanOpenAsync(ClaimsPrincipal user, Type pageType, CancellationToken ct = default)
    {
        if (TryResolveStaticPageAccess(user, pageType, out var module, out var staticAllowed))
            return staticAllowed;

        var enabled = await EnabledModulesAsync(ct);
        return EvaluateModuleAccess(pageType, module!, enabled);
    }

    private static bool TryResolveStaticPageAccess(ClaimsPrincipal user, Type pageType, out string? module, out bool allowed)
    {
        module = null;
        if (user.Identity?.IsAuthenticated != true)
        {
            allowed = true;
            return true;
        }
        if (user.IsInRole(RoleNames.AppOwner))
        {
            allowed = true;
            return true;
        }
        if (pageType.Name is "AppOwnerDashboard" or "AppOwnerInvoice")
        {
            allowed = false;
            return true;
        }
        module = LicensedModules.ForPage(pageType);
        if (module is null)
        {
            allowed = true;
            return true;
        }
        allowed = false;
        return false;
    }

    private static bool EvaluateModuleAccess(Type pageType, string module, IReadOnlySet<string> enabled)
    {
        if (pageType.Name == "PharmacyPrescriptions")
            return enabled.Contains(LicensedModules.Doctor) || enabled.Contains(LicensedModules.Pharmacy);
        return enabled.Contains(module);
    }

    public async Task<IReadOnlyList<string>> RenewalWarningsAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var license = await db.HospitalLicenses.AsNoTracking().SingleOrDefaultAsync(x => x.ID == 1, ct);
        if (license is null || !license.IsActivated) return [];
        var now = DateTime.UtcNow.Date;
        var until = now.AddDays(15);
        var warnings = new List<string>();
        if (license.HostingExpiresAt is DateTime hosting && hosting.Date <= until)
            warnings.Add($"Hosting renew by {hosting:dd MMM yyyy}.");
        if (license.DomainExpiresAt is DateTime domain && domain.Date <= until)
            warnings.Add($"Domain renew by {domain:dd MMM yyyy}.");
        if (license.SubscriptionExpiresAt is DateTime subscription && subscription.Date <= until)
            warnings.Add($"Subscription renew by {subscription:dd MMM yyyy}.");
        return warnings;
    }

    public static bool StaffRoleAvailable(string role, IReadOnlySet<string> enabled) => role switch
    {
        RoleNames.Administrator or RoleNames.AppOwner or RoleNames.HospitalOwner or RoleNames.Patient => false,
        RoleNames.Reception => enabled.Overlaps([LicensedModules.Registration, LicensedModules.Appointments, LicensedModules.Diagnostics, LicensedModules.Ipd]),
        RoleNames.Cashier => enabled.Overlaps([LicensedModules.Diagnostics, LicensedModules.Emergency, LicensedModules.Pharmacy, LicensedModules.Ipd, LicensedModules.Accounts, LicensedModules.Canteen]),
        RoleNames.Doctor => enabled.Overlaps([LicensedModules.Doctor, LicensedModules.Diagnostics, LicensedModules.Ipd, LicensedModules.Ot, LicensedModules.BloodBank]),
        RoleNames.Nurse => enabled.Overlaps([LicensedModules.Nursing, LicensedModules.Doctor, LicensedModules.BloodBank]),
        RoleNames.LabTechnician => enabled.Overlaps([LicensedModules.Diagnostics, LicensedModules.BloodBank]),
        RoleNames.PharmacyOperator => enabled.Contains(LicensedModules.Pharmacy),
        RoleNames.BloodBankOperator => enabled.Contains(LicensedModules.BloodBank),
        RoleNames.IPDOperator => enabled.Contains(LicensedModules.Ipd),
        RoleNames.EmergencyOperator => enabled.Contains(LicensedModules.Emergency),
        RoleNames.OTOperator => enabled.Contains(LicensedModules.Ot),
        RoleNames.Authorizer => enabled.Contains(LicensedModules.Accounts),
        RoleNames.StoreKeeper => enabled.Contains(LicensedModules.Store),
        _ => false
    };
}
