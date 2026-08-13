using Microsoft.AspNetCore.Components;

namespace MoopelFrontend.Shared.Helpers;

public static class RouteHelper
{
    public static string GetBaseUri(NavigationManager nav)
    {
        ArgumentNullException.ThrowIfNull(nav);

        string currentPath = "/" + nav.ToBaseRelativePath(nav.Uri);

        return currentPath;
    }
}
