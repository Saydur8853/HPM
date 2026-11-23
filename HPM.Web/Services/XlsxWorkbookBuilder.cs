using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace HPM.Web.Services;

/// <summary>
/// Builds multi-sheet .xlsx workbooks with styled headers and Excel list data-validation dropdowns
/// referencing lookup sheets, without any external spreadsheet dependencies.
/// </summary>
public static class XlsxWorkbookBuilder
{
    private static readonly XNamespace Main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly XNamespace Rel = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private static readonly XNamespace PkgRel = "http://schemas.openxmlformats.org/package/2006/relationships";
    private static readonly XNamespace Ct = "http://schemas.openxmlformats.org/package/2006/content-types";

    public sealed record DropdownValidation(int ColumnIndex, string TargetSheetName, int TargetRowCount);

    public sealed record SheetSpec(
        string Name,
        IReadOnlyList<string> Headers,
        IReadOnlyList<IReadOnlyList<string>> Rows,
        IReadOnlyList<DropdownValidation>? Validations = null);

    public static byte[] Build(IReadOnlyList<SheetSpec> sheets)
    {
        if (sheets.Count == 0)
            throw new ArgumentException("At least one sheet is required.", nameof(sheets));

        var sanitizedNames = new string[sheets.Count];
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < sheets.Count; i++)
        {
            sanitizedNames[i] = SanitizeSheetName(sheets[i].Name, used, i + 1);
        }

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            WriteXml(zip, "[Content_Types].xml", BuildContentTypes(sheets.Count));
            WriteXml(zip, "_rels/.rels", BuildRootRels());
            WriteXml(zip, "xl/workbook.xml", BuildWorkbook(sanitizedNames));
            WriteXml(zip, "xl/_rels/workbook.xml.rels", BuildWorkbookRels(sheets.Count));
            WriteXml(zip, "xl/styles.xml", BuildStyles());

            for (var i = 0; i < sheets.Count; i++)
            {
                WriteXml(zip, $"xl/worksheets/sheet{i + 1}.xml", BuildWorksheet(sheets[i]));
            }
        }
        return ms.ToArray();
    }

    private static string SanitizeSheetName(string name, HashSet<string> used, int index)
    {
        var invalid = new[] { ':', '\\', '/', '?', '*', '[', ']' };
        var cleaned = new string(name.Where(c => !invalid.Contains(c)).ToArray()).Trim();
        if (string.IsNullOrWhiteSpace(cleaned)) cleaned = $"Sheet{index}";
        if (cleaned.Length > 31) cleaned = cleaned[..31];

        var candidate = cleaned;
        var suffix = 2;
        while (!used.Add(candidate))
        {
            var tag = $"_{suffix++}";
            candidate = cleaned[..Math.Min(cleaned.Length, 31 - tag.Length)] + tag;
        }
        return candidate;
    }

    private static void WriteXml(ZipArchive zip, string path, XDocument doc)
    {
        var entry = zip.CreateEntry(path, CompressionLevel.Fastest);
        using var stream = entry.Open();
        using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        doc.Save(writer, SaveOptions.DisableFormatting);
    }

    private static XDocument BuildContentTypes(int sheetCount)
    {
        var types = new XElement(Ct + "Types",
            new XElement(Ct + "Default", new XAttribute("Extension", "rels"), new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")),
            new XElement(Ct + "Default", new XAttribute("Extension", "xml"), new XAttribute("ContentType", "application/xml")),
            new XElement(Ct + "Override", new XAttribute("PartName", "/xl/workbook.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml")),
            new XElement(Ct + "Override", new XAttribute("PartName", "/xl/styles.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml")));

        for (var i = 1; i <= sheetCount; i++)
        {
            types.Add(new XElement(Ct + "Override",
                new XAttribute("PartName", $"/xl/worksheets/sheet{i}.xml"),
                new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml")));
        }
        return new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), types);
    }

    private static XDocument BuildRootRels() => new(
        new XDeclaration("1.0", "UTF-8", "yes"),
        new XElement(PkgRel + "Relationships",
            new XElement(PkgRel + "Relationship",
                new XAttribute("Id", "rId1"),
                new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"),
                new XAttribute("Target", "xl/workbook.xml"))));

    private static XDocument BuildWorkbook(string[] sheetNames)
    {
        var sheetsEl = new XElement(Main + "sheets");
        for (var i = 0; i < sheetNames.Length; i++)
        {
            sheetsEl.Add(new XElement(Main + "sheet",
                new XAttribute("name", sheetNames[i]),
                new XAttribute("sheetId", i + 1),
                new XAttribute(Rel + "id", $"rId{i + 1}")));
        }
        return new XDocument(
            new XDeclaration("1.0", "UTF-8", "yes"),
            new XElement(Main + "workbook",
                new XAttribute(XNamespace.Xmlns + "r", Rel.NamespaceName),
                sheetsEl));
    }

    private static XDocument BuildWorkbookRels(int sheetCount)
    {
        var rels = new XElement(PkgRel + "Relationships");
        for (var i = 1; i <= sheetCount; i++)
        {
            rels.Add(new XElement(PkgRel + "Relationship",
                new XAttribute("Id", $"rId{i}"),
                new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"),
                new XAttribute("Target", $"worksheets/sheet{i}.xml")));
        }
        rels.Add(new XElement(PkgRel + "Relationship",
            new XAttribute("Id", $"rId{sheetCount + 1}"),
            new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"),
            new XAttribute("Target", "styles.xml")));
        return new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), rels);
    }

    private static XDocument BuildStyles() => new(
        new XDeclaration("1.0", "UTF-8", "yes"),
        new XElement(Main + "styleSheet",
            new XElement(Main + "fonts", new XAttribute("count", 2),
                new XElement(Main + "font",
                    new XElement(Main + "sz", new XAttribute("val", 11)),
                    new XElement(Main + "name", new XAttribute("val", "Calibri"))),
                new XElement(Main + "font",
                    new XElement(Main + "b"),
                    new XElement(Main + "sz", new XAttribute("val", 11)),
                    new XElement(Main + "color", new XAttribute("rgb", "FF0F172A")),
                    new XElement(Main + "name", new XAttribute("val", "Calibri")))),
            new XElement(Main + "fills", new XAttribute("count", 3),
                new XElement(Main + "fill", new XElement(Main + "patternFill", new XAttribute("patternType", "none"))),
                new XElement(Main + "fill", new XElement(Main + "patternFill", new XAttribute("patternType", "gray125"))),
                new XElement(Main + "fill", new XElement(Main + "patternFill",
                    new XAttribute("patternType", "solid"),
                    new XElement(Main + "fgColor", new XAttribute("rgb", "FFE6F7F5"))))),
            new XElement(Main + "borders", new XAttribute("count", 1),
                new XElement(Main + "border",
                    new XElement(Main + "left"),
                    new XElement(Main + "right"),
                    new XElement(Main + "top"),
                    new XElement(Main + "bottom"),
                    new XElement(Main + "diagonal"))),
            new XElement(Main + "cellStyleXfs", new XAttribute("count", 1),
                new XElement(Main + "xf",
                    new XAttribute("numFmtId", 0),
                    new XAttribute("fontId", 0),
                    new XAttribute("fillId", 0),
                    new XAttribute("borderId", 0))),
            new XElement(Main + "cellXfs", new XAttribute("count", 2),
                new XElement(Main + "xf",
                    new XAttribute("numFmtId", 0),
                    new XAttribute("fontId", 0),
                    new XAttribute("fillId", 0),
                    new XAttribute("borderId", 0),
                    new XAttribute("xfId", 0)),
                new XElement(Main + "xf",
                    new XAttribute("numFmtId", 0),
                    new XAttribute("fontId", 1),
                    new XAttribute("fillId", 2),
                    new XAttribute("borderId", 0),
                    new XAttribute("xfId", 0),
                    new XAttribute("applyFont", 1),
                    new XAttribute("applyFill", 1)))));

    private static XDocument BuildWorksheet(SheetSpec spec)
    {
        var colCount = spec.Headers.Count;
        var widths = new int[colCount];
        for (var c = 0; c < colCount; c++)
            widths[c] = Math.Clamp(spec.Headers[c].Length + 4, 14, 42);

        foreach (var row in spec.Rows)
        {
            for (var c = 0; c < Math.Min(colCount, row.Count); c++)
            {
                if (!string.IsNullOrEmpty(row[c]))
                    widths[c] = Math.Max(widths[c], Math.Min(46, row[c].Length + 3));
            }
        }

        var colsEl = new XElement(Main + "cols");
        for (var c = 0; c < colCount; c++)
        {
            colsEl.Add(new XElement(Main + "col",
                new XAttribute("min", c + 1),
                new XAttribute("max", c + 1),
                new XAttribute("width", widths[c]),
                new XAttribute("customWidth", 1)));
        }

        var sheetData = new XElement(Main + "sheetData");
        sheetData.Add(BuildRow(1, spec.Headers, styleId: 1));
        for (var r = 0; r < spec.Rows.Count; r++)
        {
            sheetData.Add(BuildRow(r + 2, spec.Rows[r], styleId: 0));
        }

        var ws = new XElement(Main + "worksheet",
            new XElement(Main + "sheetViews",
                new XElement(Main + "sheetView",
                    new XAttribute("workbookViewId", 0),
                    new XElement(Main + "pane",
                        new XAttribute("ySplit", 1),
                        new XAttribute("topLeftCell", "A2"),
                        new XAttribute("activePane", "bottomLeft"),
                        new XAttribute("state", "frozen")))),
            colsEl,
            sheetData);

        if (spec.Validations is { Count: > 0 })
        {
            var validationsEl = new XElement(Main + "dataValidations",
                new XAttribute("count", spec.Validations.Count));
            foreach (var v in spec.Validations)
            {
                var colLetter = ColumnLetter(v.ColumnIndex);
                var maxRow = Math.Max(2, v.TargetRowCount + 1);
                var escapedSheet = v.TargetSheetName.Replace("'", "''");
                validationsEl.Add(new XElement(Main + "dataValidation",
                    new XAttribute("type", "list"),
                    new XAttribute("allowBlank", "1"),
                    new XAttribute("showInputMessage", "1"),
                    new XAttribute("showErrorMessage", "1"),
                    new XAttribute("sqref", $"{colLetter}2:{colLetter}5000"),
                    new XElement(Main + "formula1", $"'{escapedSheet}'!$A$2:$A${maxRow}")));
            }
            ws.Add(validationsEl);
        }

        return new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), ws);
    }

    private static XElement BuildRow(int rowNumber, IReadOnlyList<string> values, int styleId)
    {
        var rowEl = new XElement(Main + "row", new XAttribute("r", rowNumber));
        for (var c = 0; c < values.Count; c++)
        {
            var cell = new XElement(Main + "c",
                new XAttribute("r", $"{ColumnLetter(c)}{rowNumber}"),
                new XAttribute("t", "inlineStr"));
            if (styleId > 0) cell.Add(new XAttribute("s", styleId));
            cell.Add(new XElement(Main + "is", new XElement(Main + "t", SanitizeXmlText(values[c] ?? ""))));
            rowEl.Add(cell);
        }
        return rowEl;
    }

    private static string SanitizeXmlText(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            if (ch == '\t' || ch == '\n' || ch == '\r' || (ch >= 0x20 && ch <= 0xD7FF) || (ch >= 0xE000 && ch <= 0xFFFD))
                sb.Append(ch);
        }
        return sb.ToString();
    }

    private static string ColumnLetter(int zeroBasedIndex)
    {
        var dividend = zeroBasedIndex + 1;
        var sb = new StringBuilder();
        while (dividend > 0)
        {
            var modulo = (dividend - 1) % 26;
            sb.Insert(0, (char)('A' + modulo));
            dividend = (dividend - modulo) / 26;
        }
        return sb.ToString();
    }
}
