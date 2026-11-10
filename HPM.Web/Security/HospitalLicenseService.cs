using System.Security.Claims;
using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Security;

public sealed class HospitalLicenseService(IDbContextFactory<HpmDbContext> dbFactory)
{
    public async Task<HashSet<string>> EnabledModulesAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var license = await db.HospitalLicenses.AsNoTracking().SingleOrDefaultAsync(x => x.ID == 1, ct);
        if (license is null)
            return LicensedModules.All.Select(x => x.Code).ToHashSet(StringComparer.OrdinalIgnoreCase); // existing installations retain access
        if (!license.IsActivated)
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var enabled = await db.HospitalModuleLicenses.AsNoTracking().Where(x => x.IsEnabled).Select(x => x.ModuleCode).ToListAsync(ct);
        return LicensedModules.WithDependencies(enabled);
    }

    public async Task<bool> CanOpenAsync(ClaimsPrincipal user, Type pageType, CancellationToken ct = default)
    {
        if (user.Identity?.IsAuthenticated != true) return true;
        if (user.IsInRole(RoleNames.AppOwner)) return true;
        if (pageType.Name is "AppOwnerDashboard" or "AppOwnerInvoice") return false;
        var module = LicensedModules.ForPage(pageType);
        if (module is null) return true;
        var enabled = await EnabledModulesAsync(ct);
        if (pageType.Name == "PharmacyPrescriptions") return enabled.Contains(LicensedModules.Doctor) || enabled.Contains(LicensedModules.Pharmacy);
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
        _ => false
    };
}
