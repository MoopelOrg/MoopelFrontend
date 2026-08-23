namespace MoopelFrontend.Shared;


public static class ConstantValues
{
    public const string BrowserAuthTokenKey = "moopel.auth.token";
    public const string AuthTokenCookieName = "moopel.auth.token";
}

/// <summary>
/// Names related to authentication state.
/// </summary>
public static class AuthConstants
{
    /// <summary>Authentication type reported on the ClaimsIdentity for a signed-in Moopel user.</summary>
    public const string AuthenticationType = "MoopelApi";
}

/// <summary>Shared UI glyphs rendered by components.</summary>
public static class UiGlyphs
{
    public const string Settings = "⚙";
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
        new("Work Items", PageRoutes.WorkItems, RequiresAuth: true),
        new("Banking", PageRoutes.Banking, RequiresAuth: true),
        new("Notes", PageRoutes.Notes, RequiresAuth: true),
        new("Tags", PageRoutes.Tags, RequiresAuth: true),
        new("Groups", PageRoutes.Groups, RequiresAuth: true),
        new("Fridge", PageRoutes.Fridges, RequiresAuth: true),
        new("Storage", PageRoutes.Storage, RequiresAuth: true),
        new("Settings", PageRoutes.Settings, RequiresAuth: true)
    ];
}

/// <summary>
/// A single app shown in the dashboard launcher grid.
/// <paramref name="Href"/> is null while the app has no page yet, which the
/// launcher renders as a disabled tile instead of a link.
/// </summary>
public sealed record AppTile(
    string Label,
    string ImageUrl,
    string GroupClass,
    string? Href = null,
    string Badge = "")
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
        new("Work Items", "images/dashboard/apps/work-items.svg", "app-tile-work-items", PageRoutes.WorkItems),
        new("Calendar", "images/dashboard/apps/calendar.svg", "app-tile-calendar"),
        new("Groups", "images/dashboard/apps/groups.svg", "app-tile-groups", PageRoutes.Groups),
        new("Fridge", "images/dashboard/apps/fridge.svg", "app-tile-fridge", PageRoutes.Fridges),
        new("Notes", "images/dashboard/apps/notes.svg", "app-tile-notes", PageRoutes.Notes),
        new("Storage", "images/dashboard/apps/storage.svg", "app-tile-storage", PageRoutes.Storage),
        new("Banking", "images/dashboard/apps/banking.svg", "app-tile-banking", PageRoutes.Banking),
        new("Documents", "images/dashboard/apps/documents.svg", "app-tile-documents"),
        new("Health", "images/dashboard/apps/health.svg", "app-tile-health"),
        new("Security", "images/dashboard/apps/security.svg", "app-tile-security")
    ];
}
