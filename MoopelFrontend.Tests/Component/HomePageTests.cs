using System.Net;

using Bunit;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

using MoopelFrontend.Tests.TestData;

using static MoopelFrontend.Tests.TestHelper;

namespace MoopelFrontend.Tests.Component;

public class HomePageTests : IClassFixture<MoopelFrontendFactory>
{
    private readonly MoopelFrontendFactory _factory;
    private readonly CancellationToken _cancelToken;

    public HomePageTests(MoopelFrontendFactory factory)
    {
        _factory = factory;
        _cancelToken = Xunit.TestContext.Current.CancellationToken;
    }

    #region Home Page

    [Fact]
    public async Task HomePage_ReturnsOk_WhenRequested()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(PageRoutes.Home, _cancelToken);

        // Assert
        Assert.True(
            condition: response.StatusCode == HttpStatusCode.OK,
            userMessage: await TMsg(HttpStatusCode.OK, response));
    }

    [Fact]
    public void HomePage_RendersHeading_WhenMounted()
    {
        // Arrange
        using Bunit.BunitContext ctx = new();
        ctx.AddAuthorization().SetNotAuthorized();

        // Act
        IRenderedComponent<MoopelFrontend.Components.Pages.Home> cut =
            ctx.Render<MoopelFrontend.Components.Pages.Home>();

        // Assert
        cut.Find("h1").MarkupMatches("<h1>Moopel</h1>");
    }

    #endregion

    #region Not Found Page

    [Fact]
    public async Task NotFoundPage_ReturnsOk_WhenRequested()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(PageRoutes.NotFound, _cancelToken);

        // Assert
        Assert.True(
            condition: response.StatusCode == HttpStatusCode.OK,
            userMessage: await TMsg(HttpStatusCode.OK, response));
    }

    #endregion
}
