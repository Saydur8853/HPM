namespace HPM.Web.Data;

public static class DiagnosticWorkflow
{
    public static bool CanAdvance(string? currentStatus, string nextStatus) =>
        (currentStatus ?? "Pending", nextStatus) is ("Pending", "InProcess") or ("InProcess", "Completed");
}
