using System.Net;
using System.Net.Sockets;

using Microsoft.AspNetCore.Builder;

namespace MoopelFrontend.Tests.Infrastructure;

public sealed class AppHostFixture : IAsyncDisposable
{
    private readonly WebApplication _app;

    private AppHostFixture(WebApplication app, string baseUrl)
    {
        _app = app;
        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; }

    public HttpClient CreateClient()
    {
        return new HttpClient
        {
            BaseAddress = new Uri(BaseUrl, UriKind.Absolute)
        };
    }

    public static async Task<AppHostFixture> StartAsync(
        string environment = "Development",
        string? apiBaseUrl = null)
    {
        int port = GetAvailablePort();
        string baseUrl = $"http://127.0.0.1:{port}";
        string contentRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MoopelFrontend"));

        List<string> arguments =
        [
            "--applicationName", typeof(Program).Assembly.GetName().Name!,
            "--contentRoot", contentRoot,
            "--environment", environment,
            "--urls", baseUrl
        ];

        if (apiBaseUrl is not null)
        {
            arguments.Add("--MoopelApiOptions:BaseUrl");
            arguments.Add(apiBaseUrl);
        }

        Startup startup = new([.. arguments]);
        startup.CreateBuilder();
        WebApplication app = startup.BuildApp();
        await app.StartAsync();

        return new AppHostFixture(app, baseUrl);
    }

    public async ValueTask DisposeAsync()
    {
        await _app.StopAsync();
        await _app.DisposeAsync();
    }

    private static int GetAvailablePort()
    {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
