using System.Security.Claims;

using Bunit;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

using MoopelFrontend.Client.Api;
using MoopelFrontend.Client.Auth;
using MoopelFrontend.Client.Models;
using MoopelFrontend.Components.Auth;
using MoopelFrontend.Components.Pages;

namespace MoopelFrontend.Tests.Component;

public class AuthFlowTests
{
    #region Login Page

    [Fact]
    public void LoginPage_RendersCredentialFields_WhenAnonymous()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService());

        // Act
        IRenderedComponent<Login> cut = ctx.Render<Login>();

        // Assert
        Assert.NotNull(cut.Find("#username"));
        Assert.NotNull(cut.Find("#password"));
    }

    [Fact]
    public void LoginPage_ShowsBackendError_WhenLoginFails()
    {
        // Arrange
        using BunitContext ctx = new();
        FakeAuthService authService = new()
        {
            LoginResultToReturn = ApiResult<LoginResult>.Fail(
                ApiErrorKind.Unauthorized,
                "Not authorized.",
                new LoginResult { Error = "Invalid username or password [testuser]" })
        };
        ctx.Services.AddSingleton<IAuthService>(authService);

        IRenderedComponent<Login> cut = ctx.Render<Login>();

        // Act
        cut.Find("#username").Change("testuser");
        cut.Find("#password").Change("wrong-password");
        cut.Find("form").Submit();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Contains("Invalid username or password", cut.Markup, StringComparison.Ordinal));
    }

    #endregion

    #region Register Page

    [Fact]
    public void RegisterPage_RendersCredentialFields_WhenAnonymous()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService());

        // Act
        IRenderedComponent<Register> cut = ctx.Render<Register>();

        // Assert
        Assert.NotNull(cut.Find("#username"));
        Assert.NotNull(cut.Find("#password"));
    }

    [Fact]
    public void RegisterPage_ShowsBackendExplanation_WhenRegistrationFails()
    {
        // Arrange
        using BunitContext ctx = new();
        FakeAuthService authService = new()
        {
            LoginResultToReturn = ApiResult<LoginResult>.Fail(
                ApiErrorKind.Validation,
                "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")
        };
        ctx.Services.AddSingleton<IAuthService>(authService);

        IRenderedComponent<Register> cut = ctx.Render<Register>();

        // Act
        cut.Find("#username").Change("newuser");
        cut.Find("#password").Change("weak");
        cut.Find("form").Submit();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Contains("Password must be at least 8 characters", cut.Markup, StringComparison.Ordinal));
    }

    #endregion

    #region Auth Gate

    [Fact]
    public void AuthGate_ShowsLoading_WhileAuthIsInitializing()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CompleteInitialization = false });

        // Act
        IRenderedComponent<AuthGate> cut = ctx.Render<AuthGate>(parameters => parameters
            .AddChildContent("<p id='app-content'>content</p>"));

        // Assert
        Assert.NotNull(cut.Find(".auth-loading"));
        Assert.Empty(cut.FindAll("#app-content"));
    }

    [Fact]
    public void AuthGate_ShowsContent_AfterAuthInitializes()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService());

        // Act
        IRenderedComponent<AuthGate> cut = ctx.Render<AuthGate>(parameters => parameters
            .AddChildContent("<p id='app-content'>content</p>"));

        // Assert
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find("#app-content")));
    }

    #endregion

    #region Auth State Provider

    [Fact]
    public async Task AuthStateProvider_ReportsAuthenticated_WhenUserIsSet()
    {
        // Arrange
        MoopelAuthStateProvider provider = new();

        // Act
        provider.SetCurrentUser(TestUser());
        AuthenticationState state = await provider.GetAuthenticationStateAsync();

        // Assert
        Assert.True(state.User.Identity?.IsAuthenticated);
        Assert.Equal("testuser", state.User.Identity?.Name);
        Assert.True(state.User.IsInRole("Standard"));
        Assert.Equal("42", state.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    [Fact]
    public async Task AuthStateProvider_ReportsAnonymous_WhenUserIsCleared()
    {
        // Arrange
        MoopelAuthStateProvider provider = new();
        provider.SetCurrentUser(TestUser());

        // Act
        provider.SetCurrentUser(null);
        AuthenticationState state = await provider.GetAuthenticationStateAsync();

        // Assert
        Assert.False(state.User.Identity?.IsAuthenticated ?? false);
    }

    #endregion

    #region User Menu

    [Fact]
    public void UserMenu_ShowsLoginLink_WhenAnonymous()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetNotAuthorized();
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService());

        // Act
        IRenderedComponent<UserMenu> cut = ctx.Render<UserMenu>();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Contains(TestData.PageRoutes.Login, cut.Find("a").GetAttribute("href"), StringComparison.Ordinal));
    }

    [Fact]
    public void UserMenu_ShowsUsernameAndLogout_WhenAuthenticated()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService());

        // Act
        IRenderedComponent<UserMenu> cut = ctx.Render<UserMenu>();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("testuser", cut.Markup, StringComparison.Ordinal);
            Assert.Contains("Log out", cut.Markup, StringComparison.Ordinal);
        });
    }

    #endregion

    private static UserRead TestUser() => new()
    {
        UserId = 42,
        Username = "testuser",
        Email = "test@example.com",
        Role = "Standard",
        CreatedAtUtc = DateTime.UtcNow,
        Deactivated = false
    };

    private sealed class FakeAuthService : IAuthService
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
}
