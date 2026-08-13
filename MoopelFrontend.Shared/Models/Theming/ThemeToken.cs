namespace MoopelFrontend.Shared.Models.Theming;

/// <summary>How a theme token's value should be edited in the UI.</summary>
public enum ThemeTokenKind
{
    /// <summary>A hex color, edited with a color picker.</summary>
    Color,

    /// <summary>A CSS length such as <c>1rem</c> or <c>6px</c>, edited as text.</summary>
    Length
}

/// <summary>Logical grouping used to lay the theme editor out in sections.</summary>
public enum ThemeTokenGroup
{
    Colors,
    Spacing,
    Sizing
}

/// <summary>
/// A single CSS custom property that a theme can override.
/// <paramref name="Variable"/> is the literal custom property name declared in <c>app.css</c>.
/// </summary>
public sealed record ThemeToken(
    string Variable,
    string Label,
    ThemeTokenKind Kind,
    ThemeTokenGroup Group,
    string DefaultValue);

/// <summary>
/// Every <c>:root</c> custom property in <c>app.css</c>, in the order they are declared there.
/// A theme is simply a value for each of these, so adding a variable to the stylesheet means
/// adding it here as well.
/// </summary>
public static class ThemeTokens
{
    public static readonly IReadOnlyList<ThemeToken> All =
    [
        new("--color-bg", "Background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f6f7f9"),
        new("--color-surface", "Surface", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ffffff"),
        new("--color-border", "Border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#d7dbe0"),
        new("--color-text", "Text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#1f2530"),
        new("--color-text-muted", "Muted text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#67707e"),
        new("--color-primary", "Primary", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#2f6fed"),
        new("--color-primary-hover", "Primary (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#2558c4"),
        new("--color-danger", "Danger", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#c62828"),
        new("--color-danger-hover", "Danger (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#a32020"),
        new("--color-success", "Success", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#26b050"),
        new("--color-error", "Error", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#e50000"),

        new("--space-xs", "Extra small", ThemeTokenKind.Length, ThemeTokenGroup.Spacing, "0.25rem"),
        new("--space-sm", "Small", ThemeTokenKind.Length, ThemeTokenGroup.Spacing, "0.5rem"),
        new("--space-md", "Medium", ThemeTokenKind.Length, ThemeTokenGroup.Spacing, "1rem"),
        new("--space-lg", "Large", ThemeTokenKind.Length, ThemeTokenGroup.Spacing, "1.5rem"),
        new("--space-xl", "Extra large", ThemeTokenKind.Length, ThemeTokenGroup.Spacing, "2.5rem"),

        new("--radius", "Corner radius", ThemeTokenKind.Length, ThemeTokenGroup.Sizing, "6px"),
        new("--content-max-width", "Content width", ThemeTokenKind.Length, ThemeTokenGroup.Sizing, "60rem"),
        new("--narrow-max-width", "Narrow content width", ThemeTokenKind.Length, ThemeTokenGroup.Sizing, "26rem"),
        new("--header-height", "Header height", ThemeTokenKind.Length, ThemeTokenGroup.Sizing, "3.5rem"),
        new("--drawer-width", "Drawer width", ThemeTokenKind.Length, ThemeTokenGroup.Sizing, "17rem")
    ];

    /// <summary>The stylesheet's own values, used as the starting point for a brand new theme.</summary>
    public static Dictionary<string, string> Defaults() =>
        All.ToDictionary(token => token.Variable, token => token.DefaultValue, StringComparer.Ordinal);
}
