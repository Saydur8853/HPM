using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace HPM.Web.Services;

/// <summary>
/// Reads the first worksheet of an .xlsx file, or a .csv file, into rows of strings.
/// The first non-empty row is the header row.
/// </summary>
public static class TabularFileReader
{
    public static List<string[]> Read(string fileName, byte[] content)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var rows = ext switch
        {
            ".xlsx" => ReadXlsx(content),
            ".csv" or ".txt" => ReadCsv(DecodeText(content)),
            _ => throw new InvalidOperationException("Only .csv and .xlsx files are supported. (.xls is not supported — save it as .xlsx or .csv first.)")
        };
        return rows.Where(r => r.Any(c => !string.IsNullOrWhiteSpace(c))).ToList();
    }

    /// <summary>Builds a UTF-8 (with BOM, so Excel shows Bangla correctly) CSV file.</summary>
    public static byte[] BuildCsv(IEnumerable<IEnumerable<string>> rows)
    {
        var sb = new StringBuilder();
        foreach (var row in rows)
            sb.Append(string.Join(",", row.Select(EscapeCsv))).Append("\r\n");
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    private static string EscapeCsv(string value) =>
        value.IndexOfAny([',', '"', '\r', '\n']) >= 0 ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;

    private static string DecodeText(byte[] content)
    {
        try
        {
            return new UTF8Encoding(false, true).GetString(content).TrimStart('﻿');
        }
        catch (DecoderFallbackException)
        {
            return Encoding.Latin1.GetString(content);
        }
    }

    private static List<string[]> ReadCsv(string text)
    {
        // Excel in some locales saves with ';' — detect from the header line.
        var firstLine = text.Split('\n', 2)[0];
        var delimiter = firstLine.Count(c => c == ';') > firstLine.Count(c => c == ',') ? ';' : ',';

        var rows = new List<string[]>();
        var row = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                    else inQuotes = false;
                }
                else field.Append(c);
            }
            else if (c == '"') inQuotes = true;
            else if (c == delimiter) { row.Add(field.ToString()); field.Clear(); }
            else if (c == '\r') { /* ignore */ }
            else if (c == '\n')
            {
                row.Add(field.ToString()); field.Clear();
                rows.Add(row.ToArray()); row.Clear();
            }
            else field.Append(c);
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row.ToArray());
        }
        return rows;
    }

    private static readonly XNamespace Main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly XNamespace RelNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private static readonly XNamespace PkgRel = "http://schemas.openxmlformats.org/package/2006/relationships";

    private static List<string[]> ReadXlsx(byte[] content)
    {
        using var zip = new ZipArchive(new MemoryStream(content), ZipArchiveMode.Read);

        var shared = new List<string>();
        var sharedEntry = zip.GetEntry("xl/sharedStrings.xml");
        if (sharedEntry is not null)
        {
            using var s = sharedEntry.Open();
            shared = XDocument.Load(s).Root!.Elements(Main + "si")
                .Select(si => string.Concat(si.Descendants(Main + "t").Select(t => t.Value)))
                .ToList();
        }

        var sheetPath = FirstSheetPath(zip);
        var sheetEntry = zip.GetEntry(sheetPath) ?? throw new InvalidOperationException("The Excel file has no worksheet.");
        using var sheetStream = sheetEntry.Open();
        var sheet = XDocument.Load(sheetStream);

        var rows = new List<string[]>();
        foreach (var r in sheet.Descendants(Main + "sheetData").Elements(Main + "row"))
        {
            var cells = new SortedDictionary<int, string>();
            var nextIndex = 0;
            foreach (var c in r.Elements(Main + "c"))
            {
                var reference = (string?)c.Attribute("r");
                var index = reference is null ? nextIndex : ColumnIndex(reference);
                nextIndex = index + 1;

                var type = (string?)c.Attribute("t");
                var raw = c.Element(Main + "v")?.Value;
                var value = type switch
                {
                    "s" when int.TryParse(raw, out var si) && si < shared.Count => shared[si],
                    "inlineStr" => string.Concat(c.Descendants(Main + "t").Select(t => t.Value)),
                    "b" => raw == "1" ? "TRUE" : "FALSE",
                    _ => raw ?? string.Empty
                };
                cells[index] = value;
            }

            if (cells.Count == 0) { rows.Add([]); continue; }
            var arr = new string[cells.Keys.Max() + 1];
            for (var i = 0; i < arr.Length; i++) arr[i] = cells.TryGetValue(i, out var v) ? v : string.Empty;
            rows.Add(arr);
        }
        return rows;
    }

    private static string FirstSheetPath(ZipArchive zip)
    {
        var workbookEntry = zip.GetEntry("xl/workbook.xml");
        var relsEntry = zip.GetEntry("xl/_rels/workbook.xml.rels");
        if (workbookEntry is not null && relsEntry is not null)
        {
            using var wb = workbookEntry.Open();
            var firstSheet = XDocument.Load(wb).Descendants(Main + "sheet").FirstOrDefault();
            var relId = (string?)firstSheet?.Attribute(RelNs + "id");
            using var rs = relsEntry.Open();
            var target = XDocument.Load(rs).Descendants(PkgRel + "Relationship")
                .FirstOrDefault(x => (string?)x.Attribute("Id") == relId)?.Attribute("Target")?.Value;
            if (!string.IsNullOrEmpty(target))
                return target.StartsWith('/') ? target.TrimStart('/') : "xl/" + target;
        }
        return "xl/worksheets/sheet1.xml";
    }

    private static int ColumnIndex(string cellReference)
    {
        var index = 0;
        foreach (var ch in cellReference)
        {
            if (!char.IsLetter(ch)) break;
            index = index * 26 + (char.ToUpperInvariant(ch) - 'A' + 1);
        }
        return index - 1;
    }

    // ── Value helpers used by importers ──

    public static bool TryParseDecimal(string? value, out decimal result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var cleaned = value.Trim().Replace(",", "").Replace("৳", "").Replace("Tk", "", StringComparison.OrdinalIgnoreCase).Trim();
        return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
    }

    public static bool TryParseDate(string? value, out DateTime result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var v = value.Trim();
        // Excel stores dates as serial numbers.
        if (double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var serial) && serial > 1 && serial < 2958465)
        {
            result = DateTime.FromOADate(serial).Date;
            return true;
        }
        string[] formats = ["yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "dd.MM.yyyy", "yyyy/MM/dd", "dd-MMM-yyyy", "d-MMM-yyyy", "dd MMM yyyy"];
        return DateTime.TryParseExact(v, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }

    public static bool? ParseBool(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim().ToLowerInvariant() switch
        {
            "yes" or "y" or "true" or "1" or "হ্যাঁ" => true,
            "no" or "n" or "false" or "0" or "না" => false,
            _ => null
        };
    }
}
