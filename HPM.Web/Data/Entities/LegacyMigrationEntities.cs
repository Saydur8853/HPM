using System.ComponentModel.DataAnnotations;

namespace HPM.Web.Data.Entities;

public sealed class LegacyMigrationRun
{
    public Guid ID { get; set; }
    [Required, StringLength(200)] public string SourceDatabase { get; set; } = string.Empty;
    [Required, StringLength(200)] public string TargetDatabase { get; set; } = string.Empty;
    [Required, StringLength(100)] public string ApprovedBy { get; set; } = string.Empty;
    [Required, StringLength(64)] public string ApprovalHash { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Status { get; set; } = "Started";
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsRollbackVerification { get; set; }
    public string? SummaryJson { get; set; }
}

public sealed class LegacyMigrationQuarantine
{
    public long ID { get; set; }
    public Guid RunID { get; set; }
    [Required, StringLength(100)] public string EntityName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string SourceID { get; set; } = string.Empty;
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    public string? PayloadJson { get; set; }
    public DateTime CreatedAt { get; set; }
}
