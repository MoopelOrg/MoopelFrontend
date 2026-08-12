using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Models;
using MoopelFrontend.Shared.Services.Interfaces;

namespace MoopelFrontend.Shared.Services;

/// <summary>Typed access to MoopelBackend's auth endpoints.</summary>
public sealed class AuthApiService : IAuthApiService
{
    private readonly IMoopelApiService _api;

    public AuthApiService(IMoopelApiService api)
    {
        _api = api;
    }

    /// <summary>
    /// Logs in. The backend returns 401 with a LoginResult body containing an Error
    /// message on bad credentials, so error bodies are read.
    /// </summary>
    public Task<ApiResult<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => _api.PostAsync<LoginResult>(ApiRoutes.Auth.Login, request, readErrorBody: true, cancellationToken);

    public Task<ApiResult<LoginResult>> GuestLoginAsync(CancellationToken cancellationToken = default)
        => _api.PostAsync<LoginResult>(ApiRoutes.Auth.GuestLogin, body: null, readErrorBody: false, cancellationToken);

    public Task<ApiResult<UserRead>> MeAsync(CancellationToken cancellationToken = default)
        => _api.GetAsync<UserRead>(ApiRoutes.Auth.Me, cancellationToken);

    /// <summary>
    /// Registers a new user. The backend returns 400 with a plain-text explanation
    /// when the username or password does not meet its rules.
    /// </summary>
    public Task<ApiResult<UserRead>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
        => _api.PostAsync<UserRead>(ApiRoutes.Auth.Register, request, readErrorBody: false, cancellationToken);
}
