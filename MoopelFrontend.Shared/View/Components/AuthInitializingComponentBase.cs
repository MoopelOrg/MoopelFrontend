using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

using MoopelFrontend.Shared.Services.Interfaces;

namespace MoopelFrontend.Shared.View.Components;

/// <summary>
/// Base class for pages that need to know the signed-in user before rendering their
/// real content. Each derived page declares its own @rendermode (so JS interop, e.g.
/// localStorage token loading, is available) and should show a loading state until
/// <see cref="IsAuthReady"/> becomes true.
/// </summary>
public abstract class AuthInitializingComponentBase : ComponentBase
{
    [Inject]
    protected IAuthService AuthService { get; set; } = default!;

    [Inject]
    private ILogger<AuthInitializingComponentBase> Logger { get; set; } = default!;

    protected bool IsAuthReady => AuthService.IsInitialized;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await AuthService.InitializeAsync();
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error Initializing AuthInitializingComponentBase");
        }
    }
}
