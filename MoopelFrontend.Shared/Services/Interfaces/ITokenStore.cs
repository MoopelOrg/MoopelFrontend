namespace MoopelFrontend.Shared.Services.Interfaces;

public interface ITokenStore
{
    string? CurrentToken { get; }
    ValueTask LoadAsync(CancellationToken cancellationToken = default);
    ValueTask SaveAsync(string token, CancellationToken cancellationToken = default);
    ValueTask ClearAsync(CancellationToken cancellationToken = default);
}
