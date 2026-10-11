namespace HPM.Web.Data.Entities;

public sealed class AuditLog
{
    public long ID { get; set; }
    public int? UserID { get; set; }
    public string UserName { get; set; } = "system";
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityID { get; set; }
    public string? ChangedFields { get; set; }
    public DateTime EventTime { get; set; } = DateTime.UtcNow;
}
