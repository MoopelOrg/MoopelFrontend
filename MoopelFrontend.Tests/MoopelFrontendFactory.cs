using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

namespace MoopelFrontend.Tests;

/// <summary>
/// Bootstraps the Blazor server-side host for integration tests,
/// mirroring the MoopelApiFactory pattern from MoopelBackend.
/// </summary>
public class MoopelFrontendFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        // Force the host to build so that all middleware and services are
        // registered before any test runs.
        CreateClient().Dispose();

        await Task.CompletedTask;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        GC.SuppressFinalize(this);

        Console.WriteLine($"Disposing MoopelFrontendFactory UTC:{DateTime.UtcNow}");
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureTestServices(_ => { });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
    }
}
