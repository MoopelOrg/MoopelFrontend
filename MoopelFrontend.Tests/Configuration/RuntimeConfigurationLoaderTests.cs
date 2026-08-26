using System.Net;

using Microsoft.Extensions.Configuration;

using MoopelFrontend.Client;

namespace MoopelFrontend.Tests.Configuration;

[TestFixture]
public sealed class RuntimeConfigurationLoaderTests
{
    [Test]
    public async Task LoadAsync_AddsRuntimeValuesAfterBuildTimeValues()
    {
        ConfigurationBuilder configuration = new();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Environment"] = "BuildTime",
            ["MoopelApiOptions:BaseUrl"] = "https://build.example/"
        });
        using HttpClient httpClient = CreateClient(
            HttpStatusCode.OK,
            """
            {
              "Environment": "Production",
              "MoopelApiOptions": {
                "BaseUrl": "https://api.example/"
              }
            }
            """);

        await RuntimeConfigurationLoader.LoadAsync(configuration, httpClient);

        IConfigurationRoot result = configuration.Build();
        Assert.Multiple(() =>
        {
            Assert.That(result["Environment"], Is.EqualTo("Production"));
            Assert.That(
                result["MoopelApiOptions:BaseUrl"],
                Is.EqualTo("https://api.example/"));
        });
    }

    [Test]
    public void LoadAsync_ThrowsWhenRuntimeConfigurationCannotBeLoaded()
    {
        ConfigurationBuilder configuration = new();
        using HttpClient httpClient = CreateClient(HttpStatusCode.ServiceUnavailable, string.Empty);

        Assert.That(
            async () => await RuntimeConfigurationLoader.LoadAsync(configuration, httpClient),
            Throws.TypeOf<HttpRequestException>());
    }

    private static HttpClient CreateClient(HttpStatusCode statusCode, string content)
    {
        return new HttpClient(new StubHttpMessageHandler(statusCode, content))
        {
            BaseAddress = new Uri("https://frontend.example/", UriKind.Absolute)
        };
    }

    private sealed class StubHttpMessageHandler(
        HttpStatusCode statusCode,
        string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content),
                RequestMessage = request
            });
        }
    }
}
