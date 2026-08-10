using Microsoft.JSInterop;

namespace MoopelFrontend.Client.Auth;

/// <summary>
/// Owns the JWT. The rest of the app never touches browser storage directly.
/// </summary>
public interface ITokenStore
{
    /// <summary>The token currently loaded in memory, if any. Attached to API requests.</summary>
    string? CurrentToken { get; }

    /// <summary>Loads any persisted token from the browser into memory.</summary>
    ValueTask LoadAsync(CancellationToken cancellationToken = default);

    ValueTask SaveAsync(string token, CancellationToken cancellationToken = default);

    ValueTask ClearAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Persists the JWT in browser localStorage via JS interop so authentication
/// survives page refreshes, and caches it in memory for request attachment.
/// </summary>
public sealed class BrowserTokenStore : ITokenStore
{
    private const string GetItemFunction = "localStorage.getItem";
    private const string SetItemFunction = "localStorage.setItem";
    private const string RemoveItemFunction = "localStorage.removeItem";

    private readonly IJSRuntime _jsRuntime;

    public BrowserTokenStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? CurrentToken { get; private set; }

    public async ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        CurrentToken = await _jsRuntime.InvokeAsync<string?>(GetItemFunction, cancellationToken, StorageKeys.AuthToken);
    }

    public async ValueTask SaveAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        CurrentToken = token;
        await _jsRuntime.InvokeVoidAsync(SetItemFunction, cancellationToken, StorageKeys.AuthToken, token);
    }

    public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        CurrentToken = null;
        await _jsRuntime.InvokeVoidAsync(RemoveItemFunction, cancellationToken, StorageKeys.AuthToken);
    }
}
