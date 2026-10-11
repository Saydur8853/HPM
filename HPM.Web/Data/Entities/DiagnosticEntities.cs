using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class DiagnosticOrder
{
    public long ID { get; set; }
    public long EncounterID { get; set; }
    [StringLength(11)] public string? PatientID { get; set; }
    [Required, StringLength(100)] public string PatientName { get; set; } = string.Empty;
    public int DoctorID { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Pending";
    [StringLength(500)] public string? ClinicalNotes { get; set; }
    public long? BilledMemoID { get; set; }
    public int OrderedBy { get; set; }
    public DateTime OrderedAt { get; set; } = DateTime.Now;
    public List<DiagnosticOrderItem> Items { get; set; } = [];
}

public sealed class DiagnosticOrderItem
{
    public long ID { get; set; }
    public long OrderID { get; set; }
    public int ServiceID { get; set; }
    public DiagnosticOrder? Order { get; set; }
}

public sealed class LabTestParameter
{
    public int ID { get; set; }
    public int ServiceID { get; set; }
    [Required, StringLength(100)] public string ParameterName { get; set; } = string.Empty;
    [StringLength(50)] public string? Unit { get; set; }
    [StringLength(200)] public string? ReferenceRange { get; set; }
    public int SortOrder { get; set; }
}

public sealed class LabSampleCollection
{
    public long ID { get; set; }
    public long MemoID { get; set; }
    public int ServiceID { get; set; }
    [StringLength(100)] public string? SpecimenType { get; set; } = "Blood";
    public DateTime? CollectedAt { get; set; }
    public int? CollectedBy { get; set; }
    [StringLength(30)] public string Status { get; set; } = "Pending";
    [StringLength(200)] public string? Remarks { get; set; }
}

public sealed class LabTestResult
{
    public long ID { get; set; }
    public long MemoID { get; set; }
    public int ServiceID { get; set; }
    [Required, StringLength(100)] public string ParameterName { get; set; } = string.Empty;
    [StringLength(100)] public string ResultValue { get; set; } = string.Empty;
    [StringLength(50)] public string? Unit { get; set; }
    [StringLength(200)] public string? ReferenceRange { get; set; }
    [StringLength(30)] public string? Flag { get; set; } = "Normal";
    [StringLength(200)] public string? Interpretation { get; set; }
    public DateTime? ReportedAt { get; set; }
    public int? ReportedBy { get; set; }
    public bool IsVerified { get; set; }
}
