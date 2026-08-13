using MoopelFrontend.Shared.Models;
using MoopelFrontend.Shared.Services.Interfaces;
using MoopelFrontend.Shared.View;

using MoopelObjects.Dto.Read;
using MoopelObjects.Requests;
using MoopelObjects.Results;

namespace MoopelFrontend.Shared.Services;

/// <summary>
/// The single centralized authentication state for the application.
/// Pages ask this service who the user is and to log in/out;
/// they never touch tokens, storage, or headers themselves.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IAuthApiService _authApi;
    private readonly ITokenStore _tokenStore;
    private readonly MoopelAuthStateProvider _stateProvider;

    public AuthService(IAuthApiService authApi, ITokenStore tokenStore,
        MoopelAuthStateProvider stateProvider, IMoopelApiService apiService)
    {
        ArgumentNullException.ThrowIfNull(apiService);

        _authApi = authApi;
        _tokenStore = tokenStore;
        _stateProvider = stateProvider;

        // Any authenticated request that comes back 401 signs the user out everywhere.
        apiService.OnUnauthorizedAsync = () => SignOutAsync();
    }

    public bool IsInitialized { get; private set; }

    public UserRead? CurrentUser => _stateProvider.CurrentUser;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
        {
            return;
        }

        try
        {
            await _tokenStore.LoadAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(_tokenStore.CurrentToken))
            {
                ApiResult<UserRead> me = await _authApi.MeAsync(cancellationToken);
                if (me.Success && me.Value is not null)
                {
                    _stateProvider.SetCurrentUser(me.Value);
                }
                // On failure the 401 path has already cleared state via OnUnauthorizedAsync;
                // for network/server errors we stay signed out without wiping the stored token,
                // so a backend blip doesn't log the user out permanently.
            }
        }
        finally
        {
            IsInitialized = true;
        }
    }

    public async Task<ApiResult<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApiResult<LoginResult> result = await _authApi.LoginAsync(request, cancellationToken);
        await ApplyLoginAsync(result, cancellationToken);
        return result;
    }

    public async Task<ApiResult<LoginResult>> GuestLoginAsync(CancellationToken cancellationToken = default)
    {
        ApiResult<LoginResult> result = await _authApi.GuestLoginAsync(cancellationToken);
        await ApplyLoginAsync(result, cancellationToken);
        return result;
    }

    public async Task<ApiResult<LoginResult>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApiResult<UserRead> registration = await _authApi.RegisterAsync(request, cancellationToken);
        if (!registration.Success)
        {
            return ApiResult<LoginResult>.Fail(registration.ErrorKind, registration.Message);
        }

        // Registration does not issue a token, so log in with the same credentials.
        LoginRequest loginRequest = new()
        {
            Username = request.Username,
            Password = request.Password
        };
        return await LoginAsync(loginRequest, cancellationToken);
    }

    /// <summary>
    /// Local sign-out only: the backend's logout endpoint is not yet implemented
    /// (AuthService.LogoutUser throws NotImplementedException). Integrate it here once it works.
    /// </summary>
    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await _tokenStore.ClearAsync(cancellationToken);
        _stateProvider.SetCurrentUser(null);
    }

    private async Task ApplyLoginAsync(ApiResult<LoginResult> result, CancellationToken cancellationToken)
    {
        if (result.Success
            && result.Value is not null
            && !string.IsNullOrWhiteSpace(result.Value.Token)
            && result.Value.User is not null)
        {
            await _tokenStore.SaveAsync(result.Value.Token, cancellationToken);
            _stateProvider.SetCurrentUser(result.Value.User);
        }
    }
}
