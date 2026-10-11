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
}
