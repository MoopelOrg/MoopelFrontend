using Microsoft.JSInterop;

using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Services.Interfaces;

namespace MoopelFrontend.Services;

/// <summary>
/// Stores the authentication token in the browser authentication cookie.
/// Uses JavaScript interop so the cookie can be modified after the Blazor
/// circuit has started.
/// </summary>
public sealed class ServerTokenStoreService : ITokenStore
{
    private const int CookieLifetimeDays = 30;

    private readonly IJSRuntime _jsRuntime;

    public ServerTokenStoreService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? CurrentToken { get; private set; }

    public async ValueTask LoadAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            CurrentToken = await _jsRuntime.InvokeAsync<string?>(
                "moopelAuthCookies.get",
                cancellationToken,
                ConstantValues.AuthTokenCookieName);
        }
        catch (JSDisconnectedException)
        {
            CurrentToken = null;
        }
    }

    public async ValueTask SaveAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "moopelAuthCookies.set",
                cancellationToken,
                ConstantValues.AuthTokenCookieName,
                token,
                CookieLifetimeDays);
        }
        catch (JSDisconnectedException)
        {
        }

        CurrentToken = token;
    }

    public async ValueTask ClearAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "moopelAuthCookies.remove",
                cancellationToken,
                ConstantValues.AuthTokenCookieName);
        }
        catch (JSDisconnectedException)
        {
        }

        CurrentToken = null;
    }
}