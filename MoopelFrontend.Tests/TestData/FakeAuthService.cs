using MoopelFrontend.Shared.Models;
using MoopelFrontend.Shared.Services.Interfaces;

using MoopelObjects.Dto.Read;
using MoopelObjects.Requests;
using MoopelObjects.Results;

namespace MoopelFrontend.Tests.TestData;

internal sealed class FakeAuthService : IAuthService
{
    public bool CompleteInitialization { get; set; } = true;
    public ApiResult<LoginResult>? LoginResultToReturn { get; set; }

    public bool IsInitialized { get; private set; }
    public UserRead? CurrentUser { get; set; }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (CompleteInitialization)
        {
            IsInitialized = true;
        }
        return Task.CompletedTask;
    }

    public Task<ApiResult<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(LoginResultToReturn
            ?? ApiResult<LoginResult>.Fail(ApiErrorKind.Unauthorized, "Not authorized."));

    public Task<ApiResult<LoginResult>> GuestLoginAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(LoginResultToReturn
            ?? ApiResult<LoginResult>.Fail(ApiErrorKind.Unauthorized, "Not authorized."));

    public Task<ApiResult<LoginResult>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(LoginResultToReturn
            ?? ApiResult<LoginResult>.Fail(ApiErrorKind.Validation, "Registration failed."));

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        CurrentUser = null;
        return Task.CompletedTask;
    }
}
