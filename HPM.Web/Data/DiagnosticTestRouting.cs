using HPM.Web.Data.Entities;

namespace HPM.Web.Data;

public static class DiagnosticTestRouting
{
    public const string Pathology = "Pathology";
    public const string Imaging = "Imaging";
    public const string Procedure = "Procedure";

    private static readonly HashSet<int> PathologyCategoryIds = [8, 41, 56, 66, 91, 101];

    private static readonly string[] ProcedureKeywords =
    [
        "ECG", "Echo", "ETT", "Holter", "EEG", "Endoscopy", "Colonoscopy", "Spirometry", "Special Procedures"
    ];

    public static string Resolve(Service service, ServiceCategory? category = null)
    {
        if (string.Equals(service.DiagnosticType, Procedure, StringComparison.OrdinalIgnoreCase)) return Procedure;
        if (string.Equals(service.DiagnosticType, Imaging, StringComparison.OrdinalIgnoreCase)) return Imaging;
        if (string.Equals(service.DiagnosticType, Pathology, StringComparison.OrdinalIgnoreCase)) return Pathology;
        return ResolveCategoryType(category);
    }

    public static string ResolveCategoryType(ServiceCategory? category)
    {
        if (category is null) return Imaging;
        if (string.Equals(category.DiagnosticType, Procedure, StringComparison.OrdinalIgnoreCase)) return Procedure;
        if (string.Equals(category.DiagnosticType, Imaging, StringComparison.OrdinalIgnoreCase)) return Imaging;
        if (string.Equals(category.DiagnosticType, Pathology, StringComparison.OrdinalIgnoreCase)) return Pathology;
        if (IsPathologyCategory(category)) return Pathology;
        if (IsProcedureCategory(category)) return Procedure;
        return Imaging;
    }

    public static bool IsPathology(Service service, ServiceCategory? category = null) => Resolve(service, category) == Pathology;
    public static bool IsImaging(Service service, ServiceCategory? category = null) => Resolve(service, category) == Imaging;
    public static bool IsProcedure(Service service, ServiceCategory? category = null) => Resolve(service, category) == Procedure;
    public static bool RequiresSampleCollection(Service service, ServiceCategory? category = null) => IsPathology(service, category);

    public static bool IsDiagnosticCategory(ServiceCategory category) =>
        !string.IsNullOrWhiteSpace(category.DiagnosticType) ||
        category.SubID == 8 || category.ID is 66 or 91;

    public static bool IsPathologyCategory(ServiceCategory? category) => category is not null &&
        (string.Equals(category.DiagnosticType, Pathology, StringComparison.OrdinalIgnoreCase) ||
         (string.IsNullOrWhiteSpace(category.DiagnosticType) &&
          (PathologyCategoryIds.Contains(category.ID) ||
           category.Name.Contains("Pathology", StringComparison.OrdinalIgnoreCase) ||
           category.Name.Contains("Laboratory", StringComparison.OrdinalIgnoreCase))));

    public static bool IsProcedureCategory(ServiceCategory? category) => category is not null &&
        (string.Equals(category.DiagnosticType, Procedure, StringComparison.OrdinalIgnoreCase) ||
         (string.IsNullOrWhiteSpace(category.DiagnosticType) &&
          ProcedureKeywords.Any(k => category.Name.Contains(k, StringComparison.OrdinalIgnoreCase))));

    public sealed record TestDiscountInfo(decimal OriginalRate, decimal DiscountAmount, decimal EffectiveRate, bool IsActive, bool IsExpired, bool IsScheduled);

    public static TestDiscountInfo EvaluateDiscount(Service service, DateTime? now = null)
    {
        var rate = Math.Max(0m, service.SaleRate ?? 0m);
        if (service.DiscountValue is not > 0m || string.IsNullOrWhiteSpace(service.DiscountType))
            return new(rate, 0m, rate, IsActive: false, IsExpired: false, IsScheduled: false);

        var current = now ?? DateTime.Now;
        var expired = service.DiscountEnd.HasValue && current > service.DiscountEnd.Value;
        var scheduled = !expired && service.DiscountStart.HasValue && current < service.DiscountStart.Value;
        if (expired || scheduled)
            return new(rate, 0m, rate, IsActive: false, IsExpired: expired, IsScheduled: scheduled);

        var discAmount = string.Equals(service.DiscountType, "Percent", StringComparison.OrdinalIgnoreCase)
            ? Math.Round(rate * Math.Clamp(service.DiscountValue.Value, 0m, 100m) / 100m, 2)
            : Math.Min(rate, Math.Max(0m, service.DiscountValue.Value));

        return new(rate, discAmount, Math.Max(0m, rate - discAmount), IsActive: discAmount > 0m, IsExpired: false, IsScheduled: false);
    }

    public static decimal GetEffectiveSaleRate(Service service, DateTime? now = null) =>
        EvaluateDiscount(service, now).EffectiveRate;

    public static string CodePrefix(Service service, ServiceCategory? category = null) => Resolve(service, category) switch
    {
        Pathology => "PAT",
        Procedure => "SPL",
        _ => "IMG"
    };

    public static string FormatCode(Service service, ServiceCategory? category = null) =>
        $"{CodePrefix(service, category)}-{service.ID}";

    public static bool TryParseCode(string? raw, out int id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(raw)) return false;
        var clean = raw.Trim().TrimStart('#');
        var dash = clean.LastIndexOf('-');
        if (dash >= 0 && dash < clean.Length - 1)
            clean = clean[(dash + 1)..];
        return int.TryParse(clean, out id) && id > 0;
    }
}
