using System.Security.Claims;
using HPM.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Security;

public static class PatientPortalAccess
{
    public static async Task<string?> CurrentPatientIdAsync(ClaimsPrincipal principal, UserManager<AppUser> users, HpmDbContext db)
    {
        if (!principal.IsInRole(RoleNames.Patient)) return null;
        var user = await users.GetUserAsync(principal);
        if (user is null || !user.IsActive || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.PhoneNumber)) return null;
        var contact = await db.Registrations.AsNoTracking()
            .Where(x => x.PatientID == user.UserName)
            .Select(x => x.Contact)
            .SingleOrDefaultAsync();
        if (!string.Equals(contact?.Trim(), user.PhoneNumber.Trim(), StringComparison.Ordinal)) return null;
        return string.Equals(principal.FindFirstValue(AppUserClaimsPrincipalFactory.PatientContactFingerprintClaim),
            AppUserClaimsPrincipalFactory.ContactFingerprint(contact!), StringComparison.Ordinal) ? user.UserName : null;
    }
}
