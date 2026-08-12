namespace MoopelFrontend.Tests.TestData;

/// <summary>
/// Centralizes all page route constants used in tests.
/// Never hardcode URL strings in test files — use this class instead.
/// Derives from the application's own PageRoutes constants so tests can never drift.
/// </summary>
public static class PageRoutes
{
    public const string Home = Shared.PageRoutes.Home;
    public const string Login = Shared.PageRoutes.Login;
    public const string Register = Shared.PageRoutes.Register;
    public const string Notes = Shared.PageRoutes.Notes;
    public const string Account = Shared.PageRoutes.Account;
    public const string NotFound = Shared.PageRoutes.NotFound;
}
