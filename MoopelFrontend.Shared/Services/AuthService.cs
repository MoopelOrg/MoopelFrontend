using Microsoft.Extensions.Logging;

using MoopelFrontend.Shared.Models;
using MoopelFrontend.Shared.Services.Interfaces;
using MoopelFrontend.Shared.View;

using MoopelObjects;
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
    private readonly IMoopelApiService _api;

    private readonly ITokenStore _tokenStore;
    private readonly MoopelAuthStateProvider _stateProvider;

    private readonly ILogger<AuthService> _logger;

    public AuthService(IMoopelApiService api, ITokenStore tokenStore, ILogger<AuthService> logger,
        MoopelAuthStateProvider stateProvider, IMoopelApiService apiService)
    {
        ArgumentNullException.ThrowIfNull(apiService);

        _api = api;
        _tokenStore = tokenStore;
        _stateProvider = stateProvider;

        _logger = logger;

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
        bool successful = true;

        try
        {
            await _tokenStore.LoadAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(_tokenStore.CurrentToken))
            {
                ApiResult<UserRead> me = await _api.GetAsync<UserRead>(ApiRoutes.Auth.Me, cancellationToken);
                if (me.Success && me.Value is not null)
                {
                    _logger.LogInformation("Loaded token and found {User}", me.Value.Username);
                    _stateProvider.SetCurrentUser(me.Value);
                }
                else
                {
                    _logger.LogInformation("Loaded token and but token was invalid or expired");
                    await ClearUserState(cancellationToken);
                }
            }
            else
            {
                _logger.LogInformation("Could not load token");
            }
        }
        catch (Exception ex)
        {
            successful = false;
            _logger.LogError(ex, "InitializeAsync error");
        }
        finally
        {
            if (successful)
            {
                if (!string.IsNullOrWhiteSpace(_stateProvider.CurrentUser?.Username))
                {
                    _logger.LogInformation("Initialized AuthService for {User}", _stateProvider.CurrentUser.Username);
                }
                IsInitialized = true;
            }
        }
    }

    public async Task<ApiResult<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApiResult<LoginResult> result = await _api.PostAsync<LoginResult>(ApiRoutes.Auth.Login, request, readErrorBody: true, cancellationToken);
        await ApplyLoginAsync(result, cancellationToken);

        return result;
    }

    public async Task<ApiResult<LoginResult>> GuestLoginAsync(CancellationToken cancellationToken = default)
    {
        ApiResult<LoginResult> result = await _api.PostAsync<LoginResult>(ApiRoutes.Auth.GuestLogin, body: null, readErrorBody: false, cancellationToken);
        await ApplyLoginAsync(result, cancellationToken);
        return result;
    }

    public async Task<ApiResult<LoginResult>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApiResult<UserRead> registration = await _api.PostAsync<UserRead>(ApiRoutes.Auth.Register, request, readErrorBody: false, cancellationToken);
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
        await ClearUserState(cancellationToken);
    }

    public async Task ClearUserState(CancellationToken cancellationToken = default)
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
