namespace MoopelFrontend.Shared;

public static class PageRoutes
{
    public const string Home = "/";
    public const string Dashboard = "/dashboard";
    public const string Login = "/login";
    public const string Register = "/register";
    public const string Notes = "/notes";
    public const string Account = "/account";
    public const string NotFound = "/not-found";

    /// <summary>Query parameter used to send the user back where they came from after login.</summary>
    public const string ReturnUrlParameter = "returnUrl";
}