using Bunit;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using MoopelFrontend.Client.View.Components;
using MoopelFrontend.Client.View.Components.Settings;
using MoopelFrontend.Client.View.Pages;
using MoopelFrontend.Shared.Models.Theming;
using MoopelFrontend.Shared.Services.Interfaces;
using MoopelFrontend.Tests.TestData;

using MoopelObjects.Dto.Read;

namespace MoopelFrontend.Tests.Component;

public class SettingsTests
{
    #region Settings Page

    [Fact]
    public void Settings_RendersEverySection_WhenAuthenticated()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CurrentUser = TestUser() });

        // Act
        IRenderedComponent<Settings> cut = ctx.Render<Settings>();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(cut.Find("#account"));
            Assert.NotNull(cut.Find("#appearance"));
            Assert.NotNull(cut.Find("#notifications"));
            Assert.NotNull(cut.Find("#security"));
            Assert.NotNull(cut.Find("#groups"));
        });
    }

    [Fact]
    public void Settings_PrefillsProfileFields_FromCurrentUser()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetAuthorized("testuser");
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService { CurrentUser = TestUser() });

        // Act
        IRenderedComponent<Settings> cut = ctx.Render<Settings>();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.Equal("testuser", cut.Find("#user-handle").GetAttribute("value"));
            Assert.Equal("test@example.com", cut.Find("#email").GetAttribute("value"));
        });
    }

    [Fact]
    public void Settings_RedirectsToLogin_WhenAnonymous()
    {
        // Arrange
        using BunitContext ctx = new();
        ctx.AddAuthorization().SetNotAuthorized();
        ctx.Services.AddSingleton<IAuthService>(new FakeAuthService());
        NavigationManager navigation = ctx.Services.GetRequiredService<NavigationManager>();

        // Act
        ctx.Render<Settings>();

        // Assert
        Assert.Contains(TestData.PageRoutes.Login, navigation.Uri, StringComparison.Ordinal);
    }

    #endregion

    #region Theme Editor

    [Fact]
    public void ThemeEditor_ListsBuiltInThemes_WhenRendered()
    {
        // Arrange
        using BunitContext ctx = new();

        // Act
        IRenderedComponent<ThemeEditor> cut = ctx.Render<ThemeEditor>();

        // Assert
        Assert.Equal(BuiltInThemes.All.Count, cut.FindAll(".theme-card").Count);
        Assert.Single(cut.FindAll(".theme-card-selected"));
    }

    [Fact]
    public void ThemeEditor_AppliesOverrideStyle_WhenNonDefaultThemeSelected()
    {
        // Arrange
        using BunitContext ctx = new();
        IRenderedComponent<ThemeEditor> cut = ctx.Render<ThemeEditor>();
        AppTheme dark = BuiltInThemes.All.First(theme => theme.Id != BuiltInThemes.DefaultThemeId);

        // Act
        cut.FindAll(".theme-card")[BuiltInThemes.All.ToList().IndexOf(dark)].Click();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Contains(dark.Values["--color-bg"], cut.Markup, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ThemeEditor_ExposesEveryToken_WhenCreatingTheme()
    {
        // Arrange
        using BunitContext ctx = new();
        IRenderedComponent<ThemeEditor> cut = ctx.Render<ThemeEditor>();

        // Act
        cut.Find(".form-actions .btn-primary").Click();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Equal(ThemeTokens.All.Count, cut.FindAll(".token-field").Count));
    }

    [Fact]
    public void ThemeEditor_AddsCustomTheme_WhenDraftSaved()
    {
        // Arrange
        using BunitContext ctx = new();
        IRenderedComponent<ThemeEditor> cut = ctx.Render<ThemeEditor>();
        cut.Find(".form-actions .btn-primary").Click();

        // Act
        cut.Find("#theme-name").Input("Midnight");
        cut.Find("#--color-primary").Change("#123456");
        cut.FindAll(".btn-primary").Last().Click();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(BuiltInThemes.All.Count + 1, cut.FindAll(".theme-card").Count);
            Assert.Contains("Midnight", cut.Markup, StringComparison.Ordinal);
            Assert.Contains("--color-primary:#123456", cut.Markup, StringComparison.Ordinal);
        });
    }

    [Fact]
    public void ThemeEditor_RejectsUnnamedTheme_WhenDraftSaved()
    {
        // Arrange
        using BunitContext ctx = new();
        IRenderedComponent<ThemeEditor> cut = ctx.Render<ThemeEditor>();
        cut.Find(".form-actions .btn-primary").Click();

        // Act
        cut.Find("#theme-name").Input(string.Empty);
        cut.FindAll(".btn-primary").Last().Click();

        // Assert
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(BuiltInThemes.All.Count, cut.FindAll(".theme-card").Count);
            Assert.NotNull(cut.Find(".form-error"));
        });
    }

    [Fact]
    public void ThemeEditor_RemovesCustomTheme_WhenDeleted()
    {
        // Arrange
        using BunitContext ctx = new();
        IRenderedComponent<ThemeEditor> cut = ctx.Render<ThemeEditor>();
        cut.Find(".form-actions .btn-primary").Click();
        cut.Find("#theme-name").Input("Midnight");
        cut.FindAll(".btn-primary").Last().Click();

        // Act
        cut.Find(".theme-card-actions .btn:last-child").Click();

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Equal(BuiltInThemes.All.Count, cut.FindAll(".theme-card").Count));
    }

    [Fact]
    public void ThemeRule_ContainsEveryToken_WhenBuilt()
    {
        // Arrange
        AppTheme theme = BuiltInThemes.Default;

        // Act
        string css = theme.ToCssRule();

        // Assert
        Assert.All(ThemeTokens.All, token =>
            Assert.Contains($"{token.Variable}:{theme.ValueOf(token)}", css, StringComparison.Ordinal));
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
