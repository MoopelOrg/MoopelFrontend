namespace MoopelFrontend.Shared;


public static class ConstantValues
{
    public const string BrowserAuthTokenKey = "moopel.auth.token";
}

/// <summary>
/// Names related to authentication state.
/// </summary>
public static class AuthConstants
{
    /// <summary>Authentication type reported on the ClaimsIdentity for a signed-in Moopel user.</summary>
    public const string AuthenticationType = "MoopelApi";
}

/// <summary>A single navigation entry rendered by the layout.</summary>
public sealed record NavItem(string Label, string Href, bool RequiresAuth);

/// <summary>
/// The application's navigation menu. The layout renders this list;
/// pages never define their own navigation.
/// </summary>
public static class AppNav
{
    public static readonly IReadOnlyList<NavItem> Items =
    [
        new("Home", PageRoutes.Home, RequiresAuth: false),
        new("Dashboard", PageRoutes.Dashboard, RequiresAuth: true),
        new("Notes", PageRoutes.Notes, RequiresAuth: true),
        new("Settings", PageRoutes.Settings, RequiresAuth: true)
    ];
}

/// <summary>
/// A single app shown in the dashboard launcher grid.
/// <paramref name="Href"/> is null while the app has no page yet, which the
/// launcher renders as a disabled tile instead of a link.
/// </summary>
public sealed record AppTile(string Label, string? Href = null, bool IsNew = false)
{
    public bool IsAvailable => !string.IsNullOrWhiteSpace(Href);
}

/// <summary>
/// The apps offered by the dashboard launcher, in display order.
/// </summary>
public static class AppLauncher
{
    public static readonly IReadOnlyList<AppTile> Tiles =
    [
        new("Work Items"),
        new("Calendar"),
        new("Groups"),
        new("Fridge"),
        new("Notes", PageRoutes.Notes, IsNew: true),
        new("Storage"),
        new("Banking"),
        new("Documents"),
        new("Health", IsNew: true),
        new("Security")
    ];
}
