using MoopelFrontend.Shared.Models;

namespace MoopelFrontend.Shared.Services.Interfaces;

public interface IAuthApiService
{
    Task<ApiResult<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<LoginResult>> GuestLoginAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<UserRead>> MeAsync(CancellationToken cancellationToken = default);
    Task<ApiResult<UserRead>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default);
}
