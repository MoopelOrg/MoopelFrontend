using MoopelFrontend.Shared.Models;

namespace MoopelFrontend.Shared.Services.Interfaces;

public interface IAuthService
{
    bool IsInitialized { get; }
    UserRead? CurrentUser { get; }
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<LoginResult>> GuestLoginAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<LoginResult>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
}
