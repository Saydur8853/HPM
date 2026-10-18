using HPM.Web.Data.Entities;

namespace HPM.Web.Data;

public static class DiagnosticTestRouting
{
    public const string Pathology = "Pathology";
    public const string Imaging = "Imaging";

    private static readonly HashSet<int> PathologyCategoryIds = [8, 41, 56, 66, 91, 101];

    public static string Resolve(Service service, ServiceCategory? category = null)
    {
        if (string.Equals(service.DiagnosticType, Imaging, StringComparison.OrdinalIgnoreCase)) return Imaging;
        if (string.Equals(service.DiagnosticType, Pathology, StringComparison.OrdinalIgnoreCase)) return Pathology;
        return IsPathologyCategory(category) ? Pathology : Imaging;
    }

    public static bool IsPathology(Service service, ServiceCategory? category = null) => Resolve(service, category) == Pathology;
    public static bool IsImaging(Service service, ServiceCategory? category = null) => Resolve(service, category) == Imaging;

    public static bool IsDiagnosticCategory(ServiceCategory category) =>
        category.SubID == 8 || category.ID is 66 or 91 ||
        category.Name.Contains("Investigation", StringComparison.OrdinalIgnoreCase) ||
        category.Name.Contains("Pathology", StringComparison.OrdinalIgnoreCase) ||
        category.Name.Contains("Laboratory", StringComparison.OrdinalIgnoreCase) ||
        category.Name.Contains("Imaging", StringComparison.OrdinalIgnoreCase) ||
        category.Name.Contains("Radiology", StringComparison.OrdinalIgnoreCase) ||
        category.Name.Contains("X-Ray", StringComparison.OrdinalIgnoreCase) ||
        category.Name.Contains("Ultrasound", StringComparison.OrdinalIgnoreCase);

    public static bool IsPathologyCategory(ServiceCategory? category) => category is not null &&
        (PathologyCategoryIds.Contains(category.ID) ||
         category.Name.Contains("Pathology", StringComparison.OrdinalIgnoreCase) ||
         category.Name.Contains("Laboratory", StringComparison.OrdinalIgnoreCase));
}
