using MoopelFrontend.Shared.Models;

namespace MoopelFrontend.Shared.Services.Interfaces;

public interface IMoopelApiService
{
    Func<Task>? OnUnauthorizedAsync { get; set; }

    Task<ApiResult<T>> GetAsync<T>(string route, CancellationToken cancellationToken = default);

    Task<ApiResult<T>> PostAsync<T>(string route, object? body,
        bool readErrorBody = false, CancellationToken cancellationToken = default);

    Task<ApiResult<bool>> DeleteAsync(string route, CancellationToken cancellationToken = default);
}
