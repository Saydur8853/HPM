using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class BloodDonor
{
    public long ID { get; set; }
    [Required, StringLength(30)] public string DonorNo { get; set; } = string.Empty;
    [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(10)] public string Sex { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18);
    [Required, StringLength(5)] public string BloodGroup { get; set; } = string.Empty;
    [Required, StringLength(30)] public string ContactNo { get; set; } = string.Empty;
    [StringLength(200)] public string? Address { get; set; }
    [StringLength(30)] public string? NationalId { get; set; }
    public decimal? WeightKg { get; set; }
    [StringLength(500)] public string? MedicalNotes { get; set; }
    public bool IsDeferred { get; set; }
    public DateTime? DeferredUntil { get; set; }
    [StringLength(300)] public string? DeferralReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int CreatedBy { get; set; }
    public List<BloodDonation> Donations { get; set; } = [];
}

public sealed class BloodDonation
{
    public long ID { get; set; }
    public long DonorID { get; set; }
    public BloodDonor? Donor { get; set; }
    [Required, StringLength(30)] public string BagNo { get; set; } = string.Empty;
    [Required, StringLength(5)] public string BloodGroup { get; set; } = string.Empty;
    public DateTime CollectedAt { get; set; } = DateTime.Now;
    public int VolumeMl { get; set; } = 450;
    [Required, StringLength(20)] public string CollectionType { get; set; } = "Whole Blood";
    [Required, StringLength(20)] public string Status { get; set; } = "Quarantine";
    [StringLength(300)] public string? Remarks { get; set; }
    public int CollectedBy { get; set; }
    public BloodScreening? Screening { get; set; }
    public List<BloodComponent> Components { get; set; } = [];
}

public sealed class BloodScreening
{
    public long ID { get; set; }
    public long DonationID { get; set; }
    public BloodDonation? Donation { get; set; }
    [Required, StringLength(15)] public string HIV { get; set; } = "Negative";
    [Required, StringLength(15)] public string HepatitisB { get; set; } = "Negative";
    [Required, StringLength(15)] public string HepatitisC { get; set; } = "Negative";
    [Required, StringLength(15)] public string Syphilis { get; set; } = "Negative";
    [Required, StringLength(15)] public string Malaria { get; set; } = "Negative";
    [Required, StringLength(15)] public string Result { get; set; } = "Negative";
    public DateTime ScreenedAt { get; set; } = DateTime.Now;
    public int ScreenedBy { get; set; }
    [StringLength(300)] public string? Remarks { get; set; }
}

public sealed class BloodComponent
{
    public long ID { get; set; }
    public long DonationID { get; set; }
    public BloodDonation? Donation { get; set; }
    [Required, StringLength(40)] public string ComponentNo { get; set; } = string.Empty;
    [Required, StringLength(30)] public string ComponentType { get; set; } = "Whole Blood";
    [Required, StringLength(5)] public string BloodGroup { get; set; } = string.Empty;
    public int VolumeMl { get; set; }
    public DateTime PreparedAt { get; set; } = DateTime.Now;
    public DateTime ExpiresAt { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Available";
    [StringLength(50)] public string? StorageLocation { get; set; }
    [StringLength(300)] public string? DiscardReason { get; set; }
    public int PreparedBy { get; set; }
}

public sealed class BloodRequisition
{
    public long ID { get; set; }
    [Required, StringLength(30)] public string RequisitionNo { get; set; } = string.Empty;
    [Required, StringLength(11)] public string PatientID { get; set; } = string.Empty;
    [Required, StringLength(120)] public string PatientName { get; set; } = string.Empty;
    [StringLength(10)] public string? RegNo { get; set; }
    [Required, StringLength(5)] public string BloodGroup { get; set; } = string.Empty;
    [Required, StringLength(30)] public string ComponentType { get; set; } = "Packed Red Cells";
    public int UnitsRequested { get; set; } = 1;
    [Required, StringLength(15)] public string Priority { get; set; } = "Routine";
    [Required, StringLength(500)] public string ClinicalIndication { get; set; } = string.Empty;
    public int? RequestedByDoctorID { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.Now;
    public DateTime? RequiredAt { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Pending";
    public int EnteredBy { get; set; }
    public List<BloodCrossMatch> CrossMatches { get; set; } = [];
}

public sealed class BloodCrossMatch
{
    public long ID { get; set; }
    public long RequisitionID { get; set; }
    public BloodRequisition? Requisition { get; set; }
    public long ComponentID { get; set; }
    public BloodComponent? Component { get; set; }
    [Required, StringLength(20)] public string Result { get; set; } = "Compatible";
    [StringLength(50)] public string? Method { get; set; } = "Gel card";
    public DateTime TestedAt { get; set; } = DateTime.Now;
    public int TestedBy { get; set; }
    [StringLength(300)] public string? Remarks { get; set; }
    public BloodIssue? Issue { get; set; }
}

public sealed class BloodIssue
{
    public long ID { get; set; }
    public long CrossMatchID { get; set; }
    public BloodCrossMatch? CrossMatch { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.Now;
    [Required, StringLength(100)] public string ReceivedBy { get; set; } = string.Empty;
    [StringLength(100)] public string? WardOrLocation { get; set; }
    public int IssuedBy { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Issued";
    public DateTime? ReturnedAt { get; set; }
    [StringLength(20)] public string? ReturnDisposition { get; set; }
    [StringLength(300)] public string? ReturnReason { get; set; }
    public int? ReturnedBy { get; set; }
}
