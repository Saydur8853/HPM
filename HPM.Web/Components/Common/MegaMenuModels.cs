namespace HPM.Web.Components.Common;

/// <summary>One link inside a <see cref="MegaMenuPanel"/> section.</summary>
public sealed record MegaMenuLink(string Label, string Href, string Icon = "", string? Subtitle = null, string? Badge = null);

/// <summary>A titled, colour-coded column of links. Accent: blue, teal, indigo, amber or rose (empty = by position).</summary>
public sealed record MegaMenuSection(string Title, string Icon, IReadOnlyList<MegaMenuLink> Links, string Accent = "");
