using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MoopelFrontend.Components.Layout;

/// <summary>
/// An <see cref="ErrorBoundary"/> that logs every caught exception before falling back
/// to <c>ErrorContent</c>. This prevents unhandled rendering/event-handling exceptions
/// from tearing down the interactive circuit (which otherwise surfaces Blazor's
/// "An unhandled error has occurred. Reload." banner) while still recording the failure.
/// </summary>
public class AppErrorBoundary : ErrorBoundary
{
    [Inject]
    private ILogger<AppErrorBoundary> Logger { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "Unhandled exception caught by {ComponentName}", nameof(AppErrorBoundary));
        return base.OnErrorAsync(exception);
    }
}
