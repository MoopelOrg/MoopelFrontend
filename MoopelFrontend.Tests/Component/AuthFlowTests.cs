using System.Security.Claims;

using Bunit;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

using MoopelFrontend.Client.Auth;
using MoopelFrontend.Client.View.Pages;
using MoopelFrontend.Shared.Models;
using MoopelFrontend.Tests.TestData;
using MoopelFrontend.View.Components.Auth;

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
}

