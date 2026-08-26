using MoopelFrontend.Shared.Models.Configuration;
using MoopelFrontend.Tests.Infrastructure;

namespace MoopelFrontend.Tests.Configuration;

[TestFixture]
public sealed class RuntimeConfigurationEndpointTests
{
    [Test]
    public async Task AppConfig_ReturnsDeploymentConfigurationWithoutCaching()
    {
        const string apiBaseUrl = "https://api.example/";
        await using AppHostFixture host = await AppHostFixture.StartAsync(
            "Production",
            apiBaseUrl);
        using HttpClient client = host.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/app-config.json");
        ClientRuntimeConfiguration? configuration =
            await response.Content.ReadFromJsonAsync<ClientRuntimeConfiguration>();

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.Headers.CacheControl?.NoStore, Is.True);
            Assert.That(configuration, Is.Not.Null);
            Assert.That(configuration!.Environment, Is.EqualTo("Production"));
            Assert.That(configuration.MoopelApiOptions.BaseUrl, Is.EqualTo(apiBaseUrl));
        });
    }
}
