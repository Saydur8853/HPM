namespace HPM.Web.Data;

public sealed class DiagnosticDailySummary
{
    public DateTime Date { get; set; }
    public int TotalTests { get; set; }
    public int PathologyCount { get; set; }
    public int PathologyAbnormalCount { get; set; }
    public double PathologyAbnormalRatio => PathologyCount > 0 ? (double)PathologyAbnormalCount / PathologyCount * 100 : 0;
    public int ImagingCount { get; set; }
    public int ImagingAbnormalCount { get; set; }
    public double ImagingAbnormalRatio => ImagingCount > 0 ? (double)ImagingAbnormalCount / ImagingCount * 100 : 0;
    public decimal Revenue { get; set; }
    public List<string> TopConditions { get; set; } = [];
}

public sealed class DiseasePrevalenceItem
{
    public string DiseaseName { get; set; } = string.Empty;
    public string DiseaseNameBn { get; set; } = string.Empty;
    public string Category { get; set; } = "Pathology"; // "Pathology" or "Imaging"
    public string TestType { get; set; } = string.Empty;
    public int EvaluatedCount { get; set; }
    public int PositiveCount { get; set; }
    public double Ratio => EvaluatedCount > 0 ? (double)PositiveCount / EvaluatedCount * 100 : 0;
    public string Severity { get; set; } = "warning"; // "critical", "warning", "info", "success"
    public string Icon { get; set; } = "🩸";
    public Dictionary<string, int> SubBreakdown { get; set; } = [];
}

public sealed class CriticalAlertItem
{
    public DateTime Time { get; set; }
    public long MemoId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? PatientId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string Modality { get; set; } = "Pathology";
    public string AbnormalValue { get; set; } = string.Empty;
    public string ClinicalAlert { get; set; } = string.Empty;
    public string Severity { get; set; } = "critical";
}
