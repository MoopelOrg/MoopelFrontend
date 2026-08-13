using Microsoft.JSInterop;

using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Services.Interfaces;

namespace MoopelFrontend.Services;

/// <summary>
/// Server-side token storage service for Interactive Server execution.
/// Stores the authentication token in protected browser storage instead of
/// modifying the HTTP response after the Blazor circuit has started.
/// </summary>
public sealed class ServerTokenStoreService : ITokenStore
{
    private const string StorageKey = ConstantValues.AuthTokenCookieName;

    private readonly IJSRuntime _jsRuntime;

    public ServerTokenStoreService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? CurrentToken { get; private set; }

    public async ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string? protectedToken =
                await _jsRuntime.InvokeAsync<string?>("moopelProtectedStorage.get", cancellationToken, StorageKey);

            if (string.IsNullOrWhiteSpace(protectedToken))
            {
                CurrentToken = null;
                return;
            }

            CurrentToken = protectedToken;
        }
        catch (JSDisconnectedException)
        {
            CurrentToken = null;
        }
    }

    public async ValueTask SaveAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        await _jsRuntime.InvokeVoidAsync(
            "moopelProtectedStorage.set",
            cancellationToken,
            StorageKey,
            token);

        CurrentToken = token;
    }

    public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        await _jsRuntime.InvokeVoidAsync(
            "moopelProtectedStorage.remove",
            cancellationToken,
            StorageKey);

        CurrentToken = null;
    }
}