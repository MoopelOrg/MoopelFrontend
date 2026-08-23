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
public sealed record AppTile(
    string Label,
    string ImageUrl,
    string ThemeStartVariable,
    string ThemeEndVariable,
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
        new("Work Items", "images/dashboard/apps/work-items.svg", "--workitems-color-primary", "--workitems-color-secondary"),
        new("Calendar", "images/dashboard/apps/calendar.svg", "--calendar-color-primary", "--calendar-color-secondary"),
        new("Groups", "images/dashboard/apps/groups.svg", "--group-color-primary", "--group-color-secondary"),
        new("Fridge", "images/dashboard/apps/fridge.svg", "--fridge-color-primary", "--fridge-color-secondary"),
        new("Notes", "images/dashboard/apps/notes.svg", "--note-color-primary", "--note-color-secondary", PageRoutes.Notes, Badge: "1"),
        new("Storage", "images/dashboard/apps/storage.svg", "--storage-color-primary", "--storage-color-secondary", Badge: "New"),
        new("Banking", "images/dashboard/apps/banking.svg", "--banking-color-primary", "--banking-color-secondary"),
        new("Documents", "images/dashboard/apps/documents.svg", "--documents-color-primary", "--documents-color-secondary"),
        new("Security", "images/dashboard/apps/security.svg", "--security-color-primary", "--security-color-secondary")
    ];
}
