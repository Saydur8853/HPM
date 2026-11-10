using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class PatientPortalConsent
{
    public long ID { get; set; }
    [Required, StringLength(11)] public string PatientID { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Version { get; set; } = "2026-11";
    public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
    public int UserID { get; set; }
}

public sealed class PatientPortalQuery
{
    public long ID { get; set; }
    [Required, StringLength(11)] public string PatientID { get; set; } = string.Empty;
    [Required, StringLength(30)] public string Topic { get; set; } = "General";
    [Required, StringLength(1000)] public string Message { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    [Required, StringLength(20)] public string Status { get; set; } = "Open";
    [StringLength(1000)] public string? Reply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public int? RepliedBy { get; set; }
}
