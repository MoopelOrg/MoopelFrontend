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
        new("--color-surface-muted", "Muted surface", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f2f4f7"),
        new("--color-border", "Border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#d7dbe0"),
        new("--color-border-strong", "Strong border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#b7c0ca"),
        new("--color-text", "Text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#1f2530"),
        new("--color-text-muted", "Muted text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#67707e"),
        new("--color-primary", "Primary", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#2f6fed"),
        new("--color-primary-hover", "Primary (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#2558c4"),
        new("--color-primary-soft", "Primary (soft)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#e8efff"),
        new("--color-primary-text", "Primary text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ffffff"),
        new("--color-danger", "Danger", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#c62828"),
        new("--color-danger-hover", "Danger (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#a32020"),
        new("--color-danger-text", "Danger text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ffffff"),
        new("--color-success", "Success", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#26b050"),
        new("--color-warning", "Warning", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ed9b00"),
        new("--color-info", "Info", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#0ea5e9"),
        new("--color-error", "Error", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#e50000"),
        new("--color-overlay", "Overlay", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "rgba(0, 0, 0, 0.4)"),
        new("--color-overlay-strong", "Overlay strong", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "rgba(0, 0, 0, 0.3)"),
        new("--color-shadow-soft", "Shadow soft", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "rgba(0, 0, 0, 0.06)"),
        new("--color-shadow-strong", "Shadow strong", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "rgba(0, 0, 0, 0.2)"),
        new("--color-shadow-modal", "Shadow modal", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "rgba(0, 0, 0, 0.3)"),
        new("--color-blazor-error-bg", "Blazor error background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#fff9c4"),
        new("--color-error-boundary-bg", "Error boundary background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#fdecea"),
        new("--color-error-boundary-border", "Error boundary border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f5c2c0"),
        new("--color-reconnect-primary", "Reconnect primary", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#6b9ed2"),
        new("--color-reconnect-primary-hover", "Reconnect primary (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#3b6ea2"),
        new("--color-reconnect-accent", "Reconnect accent", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#0087ff"),

        new("--dashboard-background", "Dashboard background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f6f7f9"),
        new("--dashboard-background-accent", "Dashboard accent background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f2f4f7"),
        new("--dashboard-header-background", "Dashboard header background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--dashboard-background) 92%, transparent)"),
        new("--dashboard-header-border", "Dashboard header border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#d7dbe0"),
        new("--dashboard-text", "Dashboard text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#1f2530"),
        new("--dashboard-text-muted", "Dashboard muted text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#67707e"),
        new("--dashboard-settings-background", "Dashboard settings background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ffffff"),
        new("--dashboard-settings-background-hover", "Dashboard settings background (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f2f4f7"),
        new("--dashboard-settings-border", "Dashboard settings border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#d7dbe0"),
        new("--dashboard-settings-border-hover", "Dashboard settings border (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#b7c0ca"),
        new("--dashboard-settings-text", "Dashboard settings text", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#67707e"),
        new("--dashboard-badge-background", "Dashboard badge background", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#2f6fed"),
        new("--dashboard-badge-border", "Dashboard badge border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ffffff"),
        new("--dashboard-surface-01", "Dashboard surface 01", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 99%, var(--color-text) 1%)"),
        new("--dashboard-surface-02", "Dashboard surface 02", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 98%, var(--color-text) 2%)"),
        new("--dashboard-surface-03", "Dashboard surface 03", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 97%, var(--color-text) 3%)"),
        new("--dashboard-surface-04", "Dashboard surface 04", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 96%, var(--color-text) 4%)"),
        new("--dashboard-surface-05", "Dashboard surface 05", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 95%, var(--color-text) 5%)"),
        new("--dashboard-surface-06", "Dashboard surface 06", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 94%, var(--color-text) 6%)"),
        new("--dashboard-surface-07", "Dashboard surface 07", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 93%, var(--color-text) 7%)"),
        new("--dashboard-surface-08", "Dashboard surface 08", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 92%, var(--color-text) 8%)"),
        new("--dashboard-surface-10", "Dashboard surface 10", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 90%, var(--color-text) 10%)"),
        new("--dashboard-surface-15", "Dashboard surface 15", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--color-surface) 85%, var(--color-text) 15%)"),
        new("--dashboard-border-soft", "Dashboard border soft", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#d7dbe0"),
        new("--dashboard-border-strong", "Dashboard border strong", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#b7c0ca"),
        new("--dashboard-focus-ring", "Dashboard focus ring", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "rgba(109, 74, 255, 0.45)"),
        new("--dashboard-accent", "Dashboard accent", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#2f6fed"),
        new("--dashboard-accent-hover", "Dashboard accent (hover)", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#2558c4"),
        new("--dashboard-accent-soft", "Dashboard accent soft", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#e8efff"),
        new("--dashboard-accent-softer", "Dashboard accent softer", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--dashboard-accent) 12%, transparent)"),
        new("--dashboard-accent-border", "Dashboard accent border", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "color-mix(in srgb, var(--dashboard-accent) 40%, transparent)"),
        new("--dashboard-overlay-strong", "Dashboard overlay strong", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "rgba(0, 0, 0, 0.6)"),

        new("--group-work-items-main-color", "Work items", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#5b5bf7"),
        new("--group-calendar-main-color", "Calendar", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#3b82f6"),
        new("--group-groups-main-color", "Groups", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#a855f7"),
        new("--group-fridge-main-color", "Fridge", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#10b981"),
        new("--group-notes-main-color", "Notes", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f59e0b"),
        new("--group-storage-main-color", "Storage", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#64748b"),
        new("--group-banking-main-color", "Banking", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f43f5e"),
        new("--group-documents-main-color", "Documents", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#0ea5e9"),
        new("--group-health-main-color", "Health", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ec4899"),
        new("--group-security-main-color", "Security", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#8b5cf6"),

        new("--tag-color-rose", "Rose", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f43f5e"),
        new("--tag-color-emerald", "Emerald", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#10b981"),
        new("--tag-color-sky", "Sky", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#0ea5e9"),
        new("--tag-color-amber", "Amber", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f59e0b"),
        new("--tag-color-purple", "Purple", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#a855f7"),
        new("--tag-color-indigo", "Indigo", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#6366f1"),
        new("--tag-color-pink", "Pink", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#ec4899"),
        new("--tag-color-orange", "Orange", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#f97316"),
        new("--tag-color-lime", "Lime", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#84cc16"),
        new("--tag-color-slate", "Slate", ThemeTokenKind.Color, ThemeTokenGroup.Colors, "#64748b"),

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


