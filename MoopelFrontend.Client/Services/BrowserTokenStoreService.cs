using Microsoft.JSInterop;

using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Services.Interfaces;

namespace MoopelFrontend.Client.Services;

/// <summary>
/// Browser token storage service. Persists the JWT in browser localStorage via JS interop
/// so authentication survives page refreshes, and caches it in memory for request attachment.
/// </summary>
public sealed class BrowserTokenStoreService : ITokenStore
{
    private const string GetItemFunction = "localStorage.getItem";
    private const string SetItemFunction = "localStorage.setItem";
    private const string RemoveItemFunction = "localStorage.removeItem";
    private const string CookieGetFunction = "moopelAuthCookies.get";
    private const string CookieSetFunction = "moopelAuthCookies.set";
    private const string CookieRemoveFunction = "moopelAuthCookies.remove";
    private const int CookieLifetimeDays = 30;

    private readonly IJSRuntime _jsRuntime;

    public BrowserTokenStoreService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? CurrentToken { get; private set; }

    public async ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        string? cookieToken = await _jsRuntime.InvokeAsync<string?>(CookieGetFunction, cancellationToken, ConstantValues.AuthTokenCookieName);
        if (!string.IsNullOrWhiteSpace(cookieToken))
        {
            CurrentToken = cookieToken;
            return;
        }

        string? localStorageToken = await _jsRuntime.InvokeAsync<string?>(GetItemFunction, cancellationToken, ConstantValues.BrowserAuthTokenKey);
        CurrentToken = localStorageToken;

        if (!string.IsNullOrWhiteSpace(localStorageToken))
        {
            await _jsRuntime.InvokeVoidAsync(CookieSetFunction, cancellationToken,
                ConstantValues.AuthTokenCookieName, localStorageToken, CookieLifetimeDays);
        }
    }

    public async ValueTask SaveAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        CurrentToken = token;
        await _jsRuntime.InvokeVoidAsync(SetItemFunction, cancellationToken, ConstantValues.BrowserAuthTokenKey, token);
        await _jsRuntime.InvokeVoidAsync(CookieSetFunction, cancellationToken,
            ConstantValues.AuthTokenCookieName, token, CookieLifetimeDays);
    }

    public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        CurrentToken = null;
        await _jsRuntime.InvokeVoidAsync(RemoveItemFunction, cancellationToken, ConstantValues.BrowserAuthTokenKey);
        await _jsRuntime.InvokeVoidAsync(CookieRemoveFunction, cancellationToken, ConstantValues.AuthTokenCookieName);
    }
}
