using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class HospitalProfile
{
    public int ID { get; set; } = 1;

    [Required, StringLength(150)]
    public string HospitalName { get; set; } = "Hospital";

    [StringLength(200)]
    public string? Tagline { get; set; } = "Excellence in Clinical Care & Diagnostic Medicine";

    [StringLength(255)]
    public string? Address { get; set; } = "Central Hospital Road, Dhaka";

    [StringLength(100)]
    public string? CityDistrict { get; set; } = "Dhaka, Bangladesh";

    [StringLength(100)]
    public string? Phone { get; set; } = "01700-000000";

    [StringLength(100)]
    public string? Hotline { get; set; } = "01700-000000";

    [StringLength(100)]
    public string? EmergencyContact { get; set; } = "10678";

    [StringLength(100)]
    public string? Email { get; set; } = "info@hospital.com";

    [StringLength(150)]
    public string? Website { get; set; } = "";

    [StringLength(100)]
    public string? LicenseNo { get; set; } = "";

    public string? LogoData { get; set; } = "";

    [StringLength(200)]
    public string? LabHeaderDept { get; set; } = "Department of Pathology, Diagnostic & Laboratory Medicine";

    [StringLength(500)]
    public string? ReceiptHeaderNote { get; set; } = "";

    [StringLength(500)]
    public string? ReceiptFooterNote { get; set; } = "";

    public bool IsSetupComplete { get; set; }
    public DateTime? SetupCompletedAt { get; set; }

    [StringLength(100)]
    public string? SetupBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }
}
