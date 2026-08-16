using Bunit;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using MoopelFrontend.Client.View.Components;
using MoopelFrontend.Client.View.Pages;
using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Services.Interfaces;
using MoopelFrontend.Shared.View.Components.Layout;
using MoopelFrontend.Tests.TestData;

using MoopelObjects.Dto.Read;

namespace MoopelFrontend.Tests.Component;

public class DashboardTests
{
    #region Dashboard Page

    [Fact]
    public void Dashboard_RendersEveryLauncherTile_WhenAuthenticated()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CurrentUser = TestUser() });

        // Act
        IRenderedComponent<Dashboard> cut = ctx.Render<Dashboard>();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Equal(AppLauncher.Tiles.Count, cut.FindAll(".app-tile").Count));
    }

    [Fact]
    public void Dashboard_LinksAvailableAppsOnly_WhenAuthenticated()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CurrentUser = TestUser() });

        // Act
        IRenderedComponent<Dashboard> cut = ctx.Render<Dashboard>();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(
                AppLauncher.Tiles.Count(tile => tile.IsAvailable),
                cut.FindAll(".app-grid a.app-tile-link").Count);
            Assert.Equal(
                AppLauncher.Tiles.Count(tile => !tile.IsAvailable),
                cut.FindAll(".app-grid .app-tile-disabled").Count);
        });
    }

    [Fact]
    public void Dashboard_RedirectsToLogin_WhenAnonymous()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetNotAuthorized();
        FakeAuthService authService = new();
        ctx.Services.AddSingleton<IAuthService>(authService);
        NavigationManager navigation = ctx.Services.GetRequiredService<NavigationManager>();

        // Act
        ctx.Render<Dashboard>();

        // Assert
        Assert.Contains(TestData.PageRoutes.Login, navigation.Uri, StringComparison.Ordinal);
    }

    #endregion

    #region Navigation Drawer

    [Fact]
    public void NavDrawer_StaysClosed_UntilToggled()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");

        // Act
        IRenderedComponent<NavDrawer> cut = ctx.Render<NavDrawer>();

        // Assert
        Assert.DoesNotContain("drawer-open", cut.Find("#app-drawer").ClassName, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll(".drawer-overlay"));
    }

    [Fact]
    public void NavDrawer_Opens_WhenToggleClicked()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        IRenderedComponent<NavDrawer> cut = ctx.Render<NavDrawer>();

        // Act
        cut.Find(".drawer-toggle").Click();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("drawer-open", cut.Find("#app-drawer").ClassName, StringComparison.Ordinal);
            Assert.NotNull(cut.Find(".drawer-overlay"));
        });
    }

    [Fact]
    public void NavDrawer_Closes_WhenOverlayClicked()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        IRenderedComponent<NavDrawer> cut = ctx.Render<NavDrawer>();
        cut.Find(".drawer-toggle").Click();

        // Act
        cut.Find(".drawer-overlay").Click();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.DoesNotContain("drawer-open", cut.Find("#app-drawer").ClassName, StringComparison.Ordinal));
    }

    [Fact]
    public void NavDrawer_ShowsAuthenticatedNavItems_WhenAuthenticated()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");

        // Act
        IRenderedComponent<NavDrawer> cut = ctx.Render<NavDrawer>();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Equal(
                AppNav.Items.Count,
                cut.FindAll(".drawer-nav-link").Count));
    }

    [Fact]
    public void NavDrawer_HidesAuthenticatedNavItems_WhenAnonymous()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetNotAuthorized();

        // Act
        IRenderedComponent<NavDrawer> cut = ctx.Render<NavDrawer>();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Equal(
                AppNav.Items.Count(item => !item.RequiresAuth),
                cut.FindAll(".drawer-nav-link").Count));
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
