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
    public string? AmbulanceTitle { get; set; } = "Ambulance";

    [StringLength(100)]
    public string? AmbulancePhone { get; set; } = "";

    [StringLength(100)]
    public string? FireServicePhone { get; set; } = "";

    [StringLength(100)]
    public string? EmergencyHotline { get; set; } = "";

    [StringLength(100)]
    public string? BloodBankPhone { get; set; } = "";

    [StringLength(150)]
    public string? PoliceStationName { get; set; } = "";

    [StringLength(100)]
    public string? PoliceStationPhone { get; set; } = "";

    [StringLength(2000)]
    public string? HospitalBranchesInfo { get; set; } = "";

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

    [StringLength(200)]
    public string? DeveloperCompanyName { get; set; } = "AsoftPro";

    [StringLength(300)]
    public string? DeveloperCopyright { get; set; } = $"Copyright © {DateTime.Now.Year} AsoftPro. All rights reserved.";

    public string? DeveloperLogoData { get; set; } = "";

    [StringLength(150)]
    public string? DeveloperEmail { get; set; } = "";

    [StringLength(100)]
    public string? DeveloperPhone { get; set; } = "";

    public bool IsSetupComplete { get; set; }
    public DateTime? SetupCompletedAt { get; set; }

    [StringLength(100)]
    public string? SetupBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }
}
