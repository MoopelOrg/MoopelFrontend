using MoopelFrontend.Shared.Services.Interfaces;

namespace MoopelFrontend.Services;

/// <summary>
/// Server-side token storage service for Interactive Server execution.
/// Keeps the token in scoped memory only (no browser storage, no JS interop).
/// </summary>
public sealed class ServerTokenStoreService : ITokenStore
{
    public string? CurrentToken { get; private set; }

    public ValueTask LoadAsync(CancellationToken cancellationToken = default)
        => ValueTask.CompletedTask;

    public ValueTask SaveAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        CurrentToken = token;
        return ValueTask.CompletedTask;
    }

    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        CurrentToken = null;
        return ValueTask.CompletedTask;
    }
}
