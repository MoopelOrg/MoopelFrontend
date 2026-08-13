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
                ["--color-border"] = "#2c2c44",
                ["--color-text"] = "#ffffff",
                ["--color-text-muted"] = "#a1a1aa",
                ["--color-primary"] = "#6d4aff",
                ["--color-primary-hover"] = "#5a3ad6",
                ["--color-danger"] = "#e5484d",
                ["--color-danger-hover"] = "#c93b40",
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
                ["--color-border"] = "#3d3d46",
                ["--color-text"] = "#f2f2f4",
                ["--color-text-muted"] = "#9a9aa6",
                ["--color-primary"] = "#e2683c",
                ["--color-primary-hover"] = "#c4562f",
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
