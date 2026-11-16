using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Security;

public sealed class AppUser : IdentityUser<int>
{
    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool MustChangePassword { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? DoctorProfileID { get; set; }

    public static bool IsGenericRoleTitle(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        var v = value.Trim();
        return v.Equals("Hospital Owner", StringComparison.OrdinalIgnoreCase)
            || v.Equals("App Owner", StringComparison.OrdinalIgnoreCase)
            || v.Equals("System Administrator", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Administrator", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Staff Member", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Staff", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Cashier", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Reception", StringComparison.OrdinalIgnoreCase)
            || RoleNames.All.Any(r => r.Equals(v, StringComparison.OrdinalIgnoreCase));
    }

    public string GetEffectiveName() =>
        !IsGenericRoleTitle(DisplayName) ? DisplayName.Trim() : (!string.IsNullOrWhiteSpace(UserName) ? UserName.Trim() : "User");

    public static string GetEffectiveName(System.Security.Claims.ClaimsPrincipal? principal, string fallback = "User")
    {
        if (principal?.Identity?.IsAuthenticated != true) return fallback;
        var displayClaim = principal.FindFirst(AppUserClaimsPrincipalFactory.DisplayNameClaim)?.Value;
        if (!IsGenericRoleTitle(displayClaim)) return displayClaim!.Trim();
        var identityName = principal.Identity?.Name;
        return !string.IsNullOrWhiteSpace(identityName) ? identityName.Trim() : fallback;
    }
}
