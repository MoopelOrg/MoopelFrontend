namespace MoopelFrontend.Shared;

public static class PageRoutes
{
    public const string Home = "/";
    public const string Dashboard = "/dashboard";
    public const string Login = "/login";
    public const string Register = "/register";
    public const string Notes = "/notes";
    public const string NewNote = "/notes/new";
    public const string Tags = "/tags";
    public const string NewTag = "/tags/new";
    public const string Settings = "/settings";
    public const string NotFound = "/not-found";

    public const string WorkItems = "/work";
    public const string NewWorkItem = "/work/new";

    public const string Banking = "/banking";
    public const string Banks = "/banking/accounts";
    public const string Transactions = "/banking/transactions";
    public const string NewTransaction = "/banking/transactions/new";

    public const string Groups = "/groups";
    public const string NewGroup = "/groups/new";

    public const string Fridges = "/fridges";
    public const string NewFridge = "/fridges/new";
    public const string NewFridgeItem = "/fridges/items/new";

    public const string Storage = "/storage";
    public const string NewStorage = "/storage/new";
    public const string NewStorageItem = "/storage/items/new";

    /// <summary>Query parameter used to send the user back where they came from after login.</summary>
    public const string ReturnUrlParameter = "returnUrl";
}
