using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HPM.Web.Security;

public sealed class AppUserClaimsPrincipalFactory(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<AppUser, IdentityRole<int>>(userManager, roleManager, optionsAccessor)
{
    public const string MustChangePasswordClaim = "must_change_password";
    public const string DoctorProfileIdClaim = "doctor_profile_id";
    public const string PatientContactFingerprintClaim = "patient_contact_fingerprint";

    public static string ContactFingerprint(string contact) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(contact.Trim())));

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (user.DoctorProfileID.HasValue)
            identity.AddClaim(new Claim(DoctorProfileIdClaim, user.DoctorProfileID.Value.ToString()));
        if (await UserManager.IsInRoleAsync(user, RoleNames.Patient) && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            identity.AddClaim(new Claim(PatientContactFingerprintClaim, ContactFingerprint(user.PhoneNumber)));
        if (user.MustChangePassword)
        {
            foreach (var roleClaim in identity.FindAll(identity.RoleClaimType).ToList())
                identity.RemoveClaim(roleClaim);
            identity.AddClaim(new Claim(MustChangePasswordClaim, bool.TrueString));
        }
        return identity;
    }
}
