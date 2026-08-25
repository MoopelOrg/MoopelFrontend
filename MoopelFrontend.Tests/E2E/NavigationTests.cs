using Microsoft.Playwright;

using MoopelFrontend.Shared;
using MoopelFrontend.Tests.Infrastructure;

namespace MoopelFrontend.Tests.E2E;

[TestFixture]
[Category("Integration")]
public sealed class NavigationTests
{
    private AppHostFixture _host = null!;
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    [SetUp]
    public async Task SetUp()
    {
        _host = await AppHostFixture.StartAsync();
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        _page.Console += (_, message) => TestContext.Progress.WriteLine($"Browser console: {message.Text}");
        _page.PageError += (_, exception) => TestContext.Progress.WriteLine($"Browser error: {exception}");
        _page.Response += (_, response) =>
        {
            if (response.Status >= 400)
            {
                TestContext.Progress.WriteLine($"HTTP {response.Status}: {response.Url}");
            }
        };
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_context is not null)
        {
            await _context.DisposeAsync();
        }

        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
        if (_host is not null)
        {
            await _host.DisposeAsync();
        }
    }

    [Test]
    public async Task HomePage_RedirectsAnonymousUserToLogin()
    {
        await _page.GotoAsync($"{_host.BaseUrl}/");
        await Assertions.Expect(_page).ToHaveURLAsync($"{_host.BaseUrl}{PageRoutes.Login}");
        await Assertions.Expect(_page.Locator("h1")).ToHaveTextAsync("Log in");
    }

    [Test]
    public async Task LoginPage_ShowsCredentialsForm()
    {
        await _page.GotoAsync($"{_host.BaseUrl}{PageRoutes.Login}");
        await Assertions.Expect(_page.Locator("#username")).ToBeVisibleAsync();
        await Assertions.Expect(_page.Locator("#password")).ToBeVisibleAsync();
    }

    [Test]
    public async Task RegisterPage_ShowsCredentialsForm()
    {
        await _page.GotoAsync($"{_host.BaseUrl}/register");
        await Assertions.Expect(_page.Locator("#username")).ToBeVisibleAsync();
        await Assertions.Expect(_page.Locator("#password")).ToBeVisibleAsync();
    }
}
