using HPM.Web.Data;
using HPM.Web.Data.Entities;
using HPM.Web.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HPM.Web.Tests;

public sealed class HospitalLicensingTests
{
    [Fact]
    public async Task App_owner_is_not_limited_by_hospital_module_license()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, RoleNames.AppOwner)], "test"));
        var service = new HospitalLicenseService(null!);
        Assert.True(await service.CanOpenAsync(user, typeof(HospitalLicensingTests)));
    }

    [Fact]
    public void Dependencies_are_added_and_cannot_be_removed_independently()
    {
        var enabled = LicensedModules.WithDependencies([LicensedModules.Diagnostics, LicensedModules.Nursing]);
        Assert.Contains(LicensedModules.Registration, enabled);
        Assert.Contains(LicensedModules.Ipd, enabled);
        Assert.DoesNotContain(LicensedModules.Appointments, enabled);
    }

    [Fact]
    public void Every_operational_route_is_classified_or_explicitly_core()
    {
        var core = new HashSet<string>(StringComparer.Ordinal)
        {
            "AccessDenied", "AppOwnerDashboard", "AppOwnerInvoice", "AuditLogs", "ChangePassword", "ClinicalMasters",
            "DoctorsMaster", "Error", "Home", "HospitalSettings", "LegacyModule", "Login", "ServiceMasters", "Setup", "UserAdmin"
        };
        var uncovered = typeof(HpmDbContext).Assembly.GetTypes()
            .Where(x => x.GetCustomAttributes(typeof(RouteAttribute), true).Length > 0)
            .Where(x => !core.Contains(x.Name) && LicensedModules.ForPage(x) is null)
            .Select(x => x.Name).OrderBy(x => x).ToList();
        Assert.Empty(uncovered);
    }

    [Fact]
    public void Monetary_columns_have_explicit_precision()
    {
        var options = new DbContextOptionsBuilder<HpmDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PrimeHpmModelOnly;Trusted_Connection=True").Options;
        using var db = new HpmDbContext(options);
        var property = db.Model.FindEntityType(typeof(HospitalSubscriptionInvoice))!.FindProperty(nameof(HospitalSubscriptionInvoice.Total))!;
        Assert.Equal(18, property.GetPrecision());
        Assert.Equal(2, property.GetScale());
    }
}
