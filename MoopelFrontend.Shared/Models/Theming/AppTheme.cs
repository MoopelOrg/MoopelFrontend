using System.Text;

namespace MoopelFrontend.Shared.Models.Theming;

/// <summary>
/// A named set of values for every <see cref="ThemeTokens"/> entry. Built-in themes ship with
/// the app and cannot be edited; user themes are created by copying one and changing values.
/// </summary>
public sealed class AppTheme
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string Description { get; init; } = string.Empty;

    public bool IsBuiltIn { get; init; }

    /// <summary>Token variable name to CSS value. Always contains every known token.</summary>
    public required Dictionary<string, string> Values { get; init; }

    public string ValueOf(ThemeToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return Values.TryGetValue(token.Variable, out string? value) ? value : token.DefaultValue;
    }

    /// <summary>Creates an editable copy, used both for "duplicate" and for starting a new theme.</summary>
    public AppTheme CopyAs(string id, string name) => new()
    {
        Id = id,
        Name = name,
        Description = Description,
        IsBuiltIn = false,
        Values = new Dictionary<string, string>(Values, StringComparer.Ordinal)
    };

    /// <summary>
    /// Renders the theme as a <c>:root</c> declaration block so it can be applied by
    /// injecting a style element, without touching <c>app.css</c> itself.
    /// </summary>
    public string ToCssRule()
    {
        StringBuilder builder = new(":root {");

        foreach (ThemeToken token in ThemeTokens.All)
        {
            builder.Append(token.Variable).Append(':').Append(ValueOf(token)).Append(';');
        }

        return builder.Append('}').ToString();
    }
}

/// <summary>The themes that ship with Moopel.</summary>
public static class BuiltInThemes
{
    public const string DefaultThemeId = "moopel-light";

    public static readonly IReadOnlyList<AppTheme> All =
    [
        new()
        {
            Id = DefaultThemeId,
            Name = "Moopel Light",
            Description = "The default look. Bright surfaces with a blue accent, tuned for daytime use.",
            IsBuiltIn = true,
            Values = ThemeTokens.Defaults()
        },
        new()
        {
            Id = "proton-dark",
            Name = "Proton Dark",
            Description = "Deep cosmic backgrounds with vibrant purple accents, designed for focus.",
            IsBuiltIn = true,
            Values = Override(new()
            {
                ["--color-bg"] = "#0a0a14",
                ["--color-surface"] = "#1a1a2e",
                ["--color-surface-muted"] = "#23233a",
                ["--color-border"] = "#2c2c44",
                ["--color-border-strong"] = "#3a3a58",
                ["--color-text"] = "#ffffff",
                ["--color-text-muted"] = "#a1a1aa",
                ["--color-primary"] = "#6d4aff",
                ["--color-primary-hover"] = "#5a3ad6",
                ["--color-primary-soft"] = "#e8efff",
                ["--color-primary-text"] = "#ffffff",
                ["--color-danger"] = "#e5484d",
                ["--color-danger-hover"] = "#c93b40",
                ["--color-danger-text"] = "#ffffff",
                ["--dashboard-background"] = "#151325",
                ["--dashboard-background-accent"] = "#1c1a2e",
                ["--dashboard-header-background"] = "color-mix(in srgb, var(--dashboard-background) 88%, transparent)",
                ["--dashboard-header-border"] = "rgba(255, 255, 255, 0.06)",
                ["--dashboard-text"] = "#ffffff",
                ["--dashboard-text-muted"] = "#9d99b9",
                ["--dashboard-settings-background"] = "rgba(255, 255, 255, 0.05)",
                ["--dashboard-settings-background-hover"] = "rgba(255, 255, 255, 0.1)",
                ["--dashboard-settings-border"] = "rgba(255, 255, 255, 0.08)",
                ["--dashboard-settings-border-hover"] = "rgba(255, 255, 255, 0.14)",
                ["--dashboard-settings-text"] = "#a4a0bd",
                ["--dashboard-badge-background"] = "#6a63ff",
                ["--dashboard-badge-border"] = "#151325",
                ["--dashboard-surface-01"] = "rgba(255, 255, 255, 0.01)",
                ["--dashboard-surface-02"] = "rgba(255, 255, 255, 0.02)",
                ["--dashboard-surface-03"] = "rgba(255, 255, 255, 0.03)",
                ["--dashboard-surface-04"] = "rgba(255, 255, 255, 0.04)",
                ["--dashboard-surface-05"] = "rgba(255, 255, 255, 0.05)",
                ["--dashboard-surface-06"] = "rgba(255, 255, 255, 0.06)",
                ["--dashboard-surface-07"] = "rgba(255, 255, 255, 0.07)",
                ["--dashboard-surface-08"] = "rgba(255, 255, 255, 0.08)",
                ["--dashboard-surface-10"] = "rgba(255, 255, 255, 0.1)",
                ["--dashboard-surface-15"] = "rgba(255, 255, 255, 0.15)",
                ["--dashboard-border-soft"] = "rgba(255, 255, 255, 0.05)",
                ["--dashboard-border-strong"] = "rgba(255, 255, 255, 0.1)",
                ["--dashboard-focus-ring"] = "rgba(109, 74, 255, 0.45)",
                ["--dashboard-accent"] = "#6d4aff",
                ["--dashboard-accent-hover"] = "#5a3ad6",
                ["--dashboard-accent-soft"] = "#e8efff",
                ["--dashboard-accent-softer"] = "color-mix(in srgb, var(--dashboard-accent) 12%, transparent)",
                ["--dashboard-accent-border"] = "color-mix(in srgb, var(--dashboard-accent) 40%, transparent)",
                ["--dashboard-overlay-strong"] = "rgba(0, 0, 0, 0.6)",
                ["--tags-panel-background"] = "rgba(255, 255, 255, 0.05)",
                ["--tags-panel-background-strong"] = "rgba(255, 255, 255, 0.08)",
                ["--tags-panel-border"] = "rgba(255, 255, 255, 0.06)",
                ["--tags-panel-border-strong"] = "rgba(255, 255, 255, 0.1)",
                ["--tags-hover-background"] = "rgba(255, 255, 255, 0.04)",
                ["--tags-input-background"] = "rgba(255, 255, 255, 0.05)",
                ["--tags-input-border"] = "rgba(255, 255, 255, 0.1)",
                ["--tags-input-border-focus"] = "rgba(109, 74, 255, 0.45)",
                ["--tags-dot-shadow-opacity"] = "0.45",
                ["--radius"] = "10px"
            })
        },
        new()
        {
            Id = "classic-slate",
            Name = "Classic Slate",
            Description = "A muted neutral palette with warm accents and tighter corners.",
            IsBuiltIn = true,
            Values = Override(new()
            {
                ["--color-bg"] = "#1e1e22",
                ["--color-surface"] = "#2a2a30",
                ["--color-surface-muted"] = "#323238",
                ["--color-border"] = "#3d3d46",
                ["--color-border-strong"] = "#4a4a54",
                ["--color-text"] = "#f2f2f4",
                ["--color-text-muted"] = "#9a9aa6",
                ["--color-primary"] = "#e2683c",
                ["--color-primary-hover"] = "#c4562f",
                ["--color-primary-soft"] = "#ffe9e0",
                ["--color-primary-text"] = "#ffffff",
                ["--dashboard-background"] = "#1e1e22",
                ["--dashboard-background-accent"] = "#2a2a30",
                ["--dashboard-header-background"] = "color-mix(in srgb, var(--dashboard-background) 88%, transparent)",
                ["--dashboard-header-border"] = "rgba(255, 255, 255, 0.05)",
                ["--dashboard-text"] = "#f2f2f4",
                ["--dashboard-text-muted"] = "#9a9aa6",
                ["--dashboard-settings-background"] = "rgba(255, 255, 255, 0.05)",
                ["--dashboard-settings-background-hover"] = "rgba(255, 255, 255, 0.1)",
                ["--dashboard-settings-border"] = "rgba(255, 255, 255, 0.08)",
                ["--dashboard-settings-border-hover"] = "rgba(255, 255, 255, 0.14)",
                ["--dashboard-settings-text"] = "#d0d0d6",
                ["--dashboard-badge-background"] = "#e2683c",
                ["--dashboard-badge-border"] = "#1e1e22",
                ["--dashboard-surface-01"] = "rgba(255, 255, 255, 0.01)",
                ["--dashboard-surface-02"] = "rgba(255, 255, 255, 0.02)",
                ["--dashboard-surface-03"] = "rgba(255, 255, 255, 0.03)",
                ["--dashboard-surface-04"] = "rgba(255, 255, 255, 0.04)",
                ["--dashboard-surface-05"] = "rgba(255, 255, 255, 0.05)",
                ["--dashboard-surface-06"] = "rgba(255, 255, 255, 0.06)",
                ["--dashboard-surface-07"] = "rgba(255, 255, 255, 0.07)",
                ["--dashboard-surface-08"] = "rgba(255, 255, 255, 0.08)",
                ["--dashboard-surface-10"] = "rgba(255, 255, 255, 0.1)",
                ["--dashboard-surface-15"] = "rgba(255, 255, 255, 0.15)",
                ["--dashboard-border-soft"] = "rgba(255, 255, 255, 0.05)",
                ["--dashboard-border-strong"] = "rgba(255, 255, 255, 0.1)",
                ["--dashboard-focus-ring"] = "rgba(226, 104, 60, 0.45)",
                ["--dashboard-accent"] = "#e2683c",
                ["--dashboard-accent-hover"] = "#c4562f",
                ["--dashboard-accent-soft"] = "#ffe9e0",
                ["--dashboard-accent-softer"] = "color-mix(in srgb, var(--dashboard-accent) 12%, transparent)",
                ["--dashboard-accent-border"] = "color-mix(in srgb, var(--dashboard-accent) 40%, transparent)",
                ["--dashboard-overlay-strong"] = "rgba(0, 0, 0, 0.6)",
                ["--tags-panel-background"] = "rgba(255, 255, 255, 0.05)",
                ["--tags-panel-background-strong"] = "rgba(255, 255, 255, 0.08)",
                ["--tags-panel-border"] = "rgba(255, 255, 255, 0.06)",
                ["--tags-panel-border-strong"] = "rgba(255, 255, 255, 0.1)",
                ["--tags-hover-background"] = "rgba(255, 255, 255, 0.04)",
                ["--tags-input-background"] = "rgba(255, 255, 255, 0.05)",
                ["--tags-input-border"] = "rgba(255, 255, 255, 0.1)",
                ["--tags-input-border-focus"] = "rgba(226, 104, 60, 0.45)",
                ["--tags-dot-shadow-opacity"] = "0.45",
                ["--radius"] = "4px"
            })
        }
    ];

    public static AppTheme Default => All[0];

    public static AppTheme? FindById(string id) =>
        All.FirstOrDefault(theme => string.Equals(theme.Id, id, StringComparison.Ordinal));

    private static Dictionary<string, string> Override(Dictionary<string, string> overrides)
    {
        Dictionary<string, string> values = ThemeTokens.Defaults();

        foreach ((string variable, string value) in overrides)
        {
            values[variable] = value;
        }

        return values;
    }
}
