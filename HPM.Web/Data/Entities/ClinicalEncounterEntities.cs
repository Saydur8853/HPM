using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public static class EncounterStatuses
{
    public const string Waiting = "Waiting";
    public const string InConsultation = "InConsultation";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
    public const string NoShow = "NoShow";

    public static readonly string[] All = [Waiting, InConsultation, Completed, Cancelled, NoShow];
}

public sealed class PatientEncounter
{
    public long ID { get; set; }
    [StringLength(11)] public string? PatientID { get; set; }
    [Required, StringLength(100)] public string PatientName { get; set; } = string.Empty;
    [StringLength(20)] public string? Age { get; set; }
    [StringLength(10)] public string? Sex { get; set; }
    public int DoctorID { get; set; }
    public int? DepartmentID { get; set; }
    public int? AppointmentID { get; set; }
    public long? TicketID { get; set; }
    [Required, StringLength(20)] public string VisitType { get; set; } = "OPD";
    [Required, StringLength(30)] public string Status { get; set; } = EncounterStatuses.Waiting;
    public int QueueNo { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.Today;
    public DateTime CheckedInAt { get; set; } = DateTime.Now;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    [StringLength(20)] public string? BloodPressure { get; set; }
    [StringLength(20)] public string? Pulse { get; set; }
    [StringLength(20)] public string? Temperature { get; set; }
    [StringLength(20)] public string? OxygenSaturation { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    [StringLength(500)] public string? Allergies { get; set; }
    [StringLength(1000)] public string? ChiefComplaint { get; set; }
    [StringLength(2000)] public string? ClinicalNotes { get; set; }
    [StringLength(1000)] public string? Diagnosis { get; set; }
    [StringLength(1000)] public string? Advice { get; set; }
    [Required, StringLength(20)] public string Priority { get; set; } = "Normal";
    public bool FollowUpRequired { get; set; }
    public DateTime? FollowUpDate { get; set; }
    [StringLength(500)] public string? FollowUpReason { get; set; }
    public DateTime? FollowUpCompletedAt { get; set; }
    public DateTime? DraftSavedAt { get; set; }
    public int CreatedBy { get; set; }
    public int? StartedBy { get; set; }
    public int? CompletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
