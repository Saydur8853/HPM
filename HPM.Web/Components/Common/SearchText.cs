using Microsoft.AspNetCore.Components;

namespace HPM.Web.Components.Common;

/// <summary>Multi-word search helpers shared by list filters and type-ahead pickers.</summary>
public static class SearchText
{
    public static string[] Tokenize(string? query) => string.IsNullOrWhiteSpace(query)
        ? []
        : query.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

    /// <summary>True when every token appears in at least one of the fields.</summary>
    public static bool MatchesAll(string[] tokens, params string?[] fields)
    {
        if (tokens.Length == 0) return true;
        var hay = string.Join(' ', fields.Where(f => !string.IsNullOrWhiteSpace(f)));
        return tokens.All(t => hay.Contains(t, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Renders <paramref name="text"/> with every token wrapped in &lt;mark&gt;.</summary>
    public static RenderFragment Highlight(string? text, string[] tokens) => builder =>
    {
        if (string.IsNullOrEmpty(text)) return;
        if (tokens.Length == 0) { builder.AddContent(0, text); return; }
        var mask = new bool[text.Length];
        foreach (var t in tokens)
        {
            var i = 0;
            while ((i = text.IndexOf(t, i, StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                for (var k = i; k < i + t.Length; k++) mask[k] = true;
                i += Math.Max(1, t.Length);
            }
        }
        var seq = 0;
        var start = 0;
        while (start < text.Length)
        {
            var hit = mask[start];
            var end = start;
            while (end < text.Length && mask[end] == hit) end++;
            if (hit)
            {
                builder.OpenElement(seq++, "mark");
                builder.AddContent(seq++, text[start..end]);
                builder.CloseElement();
            }
            else builder.AddContent(seq++, text[start..end]);
            start = end;
        }
    };
}
