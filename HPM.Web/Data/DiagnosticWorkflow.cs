namespace HPM.Web.Data;

public static class DiagnosticWorkflow
{
    public static bool CanAdvance(string? currentStatus, string nextStatus) =>
        (currentStatus ?? "Pending", nextStatus) is
            ("Pending", "Collected") or
            ("Collected", "Received") or
            ("Received", "InProcess") or
            ("Pending", "Scheduled") or
            ("Scheduled", "InProcess") or
            ("Pending", "InProcess") or
            ("InProcess", "Completed");
}
