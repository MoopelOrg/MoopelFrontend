namespace MoopelFrontend.Shared;

/// <summary>
/// Frontend page routes. Never hardcode page URL strings — use these.
/// Mirrors the Routes pattern from MoopelBackend.
/// </summary>
public static class PageRoutes
{
    public const string Home = "/";
    public const string Login = "/login";
    public const string Register = "/register";
    public const string Notes = "/notes";
    public const string Account = "/account";
    public const string NotFound = "/not-found";

    /// <summary>Query parameter used to send the user back where they came from after login.</summary>
    public const string ReturnUrlParameter = "returnUrl";
}

/// <summary>
/// MoopelBackend API routes. Must match MoopelApi's Routes.cs exactly.
/// </summary>
public static class ApiRoutes
{
    public static class Auth
    {
        private const string Base = "auth";

        public const string Login = Base + "/login";
        public const string GuestLogin = Base + "/guest-login";
        public const string Logout = Base + "/logout";
        public const string Register = Base + "/register";
        public const string Me = Base + "/me";
    }

    public static class Note
    {
        private const string Base = "note";

        public const string MyNotes = Base + "/my";
        public const string CreateNote = Base + "/create";

        public static string NoteById(int noteId) => $"{Base}/{noteId}";
        public static string DeleteNote(int noteId) => $"{Base}/delete/{noteId}";
    }
}

/// <summary>
/// Keys used for browser storage. Never hardcode storage key strings.
/// </summary>
public static class StorageKeys
{
    public const string AuthToken = "moopel.auth.token";
}

/// <summary>
/// Configuration section names. Values themselves live in appsettings.
/// </summary>
public static class ConfigSections
{
    public const string MoopelApi = "MoopelApi";
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
        new("Notes", PageRoutes.Notes, RequiresAuth: true),
        new("Account", PageRoutes.Account, RequiresAuth: true)
    ];
}
