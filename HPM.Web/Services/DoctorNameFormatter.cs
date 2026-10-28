namespace HPM.Web.Services;

using System.Text.RegularExpressions;

public static class DoctorNameFormatter
{
    /// <summary>
    /// Sanitizes doctor names and titles by removing unwanted dots, filler periods, leading dots, and normalizing spaces.
    /// E.g. "........Sahla Mahjabin" -> "Sahla Mahjabin", "....Dr.Md.Al" -> "Dr. Md. Al", "......AMITAV SAHA " -> "AMITAV SAHA"
    /// </summary>
    public static string Sanitize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var trimmed = input.Trim();
        if (trimmed.All(c => c == '.' || char.IsWhiteSpace(c)))
            return string.Empty;

        // Strip leading dots and spaces, e.g. "....Dr.Md" -> "Dr.Md", "...M. S." -> "M. S.", "........Sahla" -> "Sahla"
        var clean = Regex.Replace(trimmed, @"^[.\s]+", "");

        // If dots followed by dot: e.g. "Dr.." or "Dr. ...." -> "Dr. "
        clean = Regex.Replace(clean, @"(?<=\.)\s*\.{1,}\s*", " ");

        // Replace any remaining 2+ consecutive dots with a space
        clean = Regex.Replace(clean, @"\s*\.{2,}\s*", " ");

        // Strip trailing multiple dots
        clean = Regex.Replace(clean, @"\.{2,}$", "");

        // Ensure space after abbreviation dots: e.g. Dr.Md.Al -> Dr. Md. Al
        clean = Regex.Replace(clean, @"(?<=\b(?:Dr|Prof|Md|Asst|Asso))\.(?=[A-Za-z])", ". ");

        // Collapse multiple spaces
        clean = Regex.Replace(clean, @"\s{2,}", " ").Trim();
        return clean;
    }

    /// <summary>
    /// Formats a complete doctor display name from Title and Name, avoiding duplicate honorifics.
    /// E.g. Title="Prof. Dr.", Name="...M. S. Zaman Shaheen" -> "Prof. Dr. M. S. Zaman Shaheen"
    /// </summary>
    public static string FormatFullName(string? title, string? name)
    {
        var cleanTitle = Sanitize(title);
        var cleanName = Sanitize(name);

        if (string.IsNullOrWhiteSpace(cleanTitle)) return cleanName;
        if (string.IsNullOrWhiteSpace(cleanName)) return cleanTitle;

        // If cleanName already starts with the title or "Dr.", avoid repeating: e.g. Title="Dr.", Name="Dr. Biddut"
        if (cleanName.StartsWith(cleanTitle, StringComparison.OrdinalIgnoreCase) ||
            (cleanTitle.EndsWith("Dr.", StringComparison.OrdinalIgnoreCase) && cleanName.StartsWith("Dr.", StringComparison.OrdinalIgnoreCase)))
        {
            return cleanName;
        }

        return $"{cleanTitle} {cleanName}".Trim();
    }
}
