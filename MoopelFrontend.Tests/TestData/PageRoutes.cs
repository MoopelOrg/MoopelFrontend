namespace MoopelFrontend.Tests.TestData;

/// <summary>
/// Centralizes all page route constants used in tests.
/// Never hardcode URL strings in test files — use this class instead.
/// Derives from the application's own PageRoutes constants so tests can never drift.
/// </summary>
public static class PageRoutes
{
    public const string Home = MoopelFrontend.Client.PageRoutes.Home;
    public const string Login = MoopelFrontend.Client.PageRoutes.Login;
    public const string Register = MoopelFrontend.Client.PageRoutes.Register;
    public const string Notes = MoopelFrontend.Client.PageRoutes.Notes;
    public const string Account = MoopelFrontend.Client.PageRoutes.Account;
    public const string NotFound = MoopelFrontend.Client.PageRoutes.NotFound;
}
