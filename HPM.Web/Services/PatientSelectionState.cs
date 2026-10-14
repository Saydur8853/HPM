namespace HPM.Web.Services;

/// <summary>
/// Keeps the selected patient available while the user moves between
/// patient-facing modules in the same interactive server circuit.
/// </summary>
public sealed class PatientSelectionState
{
    public string? PatientId { get; private set; }

    public void Select(string? patientId)
    {
        PatientId = string.IsNullOrWhiteSpace(patientId) ? null : patientId.Trim();
    }

    public void Clear() => PatientId = null;
}
