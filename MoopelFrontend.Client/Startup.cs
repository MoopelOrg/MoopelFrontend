using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

using MoopelFrontend.Client.Services;
using MoopelFrontend.Shared.Models.Configuration;
using MoopelFrontend.Shared.Services;
using MoopelFrontend.Shared.Services.Interfaces;
using MoopelFrontend.Shared.View;

using Serilog;

namespace MoopelFrontend.Client;

public sealed class Startup
{
    private WebAssemblyHostBuilder _builder;

    public Startup(string[] args)
    {
        _builder = WebAssemblyHostBuilder.CreateDefault(args);
    }

    public WebAssemblyHostBuilder CreateBuilder()
    {
        _builder = AddLogging();
        _builder = AddServices();
        _builder = AddLifetimeServices();

        return _builder;
    }

    public WebAssemblyHostBuilder AddLogging()
    {
        _builder.Logging.ClearProviders();

        LoggerConfiguration config = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
            .WriteTo.Http(requestUri: "/api/clientlog", queueLimitBytes: null);

        if (_builder.HostEnvironment.IsDevelopment())
            config = config.WriteTo.BrowserConsole();

        Log.Logger = config.CreateLogger();

        _builder.Logging.AddSerilog(Log.Logger);

        return _builder;
    }

    public WebAssemblyHostBuilder AddServices()
    {
        return _builder;
    }

    public WebAssemblyHostBuilder AddLifetimeServices()
    {
        _builder.Services.AddOptions<MoopelApiOptions>()
            .Bind(_builder.Configuration.GetSection(nameof(MoopelApiOptions)));

        _builder.Services.AddAuthorizationCore();
        _builder.Services.AddCascadingAuthenticationState();

        _builder.Services.AddScoped<ITokenStore, BrowserTokenStoreService>();
        _builder.Services.AddScoped<MoopelAuthStateProvider>();
        _builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MoopelAuthStateProvider>());

        _builder.Services.AddHttpClient<IMoopelApiService, MoopelApiService>((sp, client) =>
        {
            MoopelApiOptions options = sp.GetRequiredService<IOptions<MoopelApiOptions>>().Value;
            ILogger<Startup> logger = sp.GetRequiredService<ILogger<Startup>>();

            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out Uri? apiBaseUri))
            {
                IWebAssemblyHostEnvironment environment = sp.GetRequiredService<IWebAssemblyHostEnvironment>();
                apiBaseUri = new Uri(environment.BaseAddress, UriKind.Absolute);
                logger.LogWarning("Invalid '{Section}:{Key}' value '{Value}'. Falling back to host base address {FallbackBaseUrl}.",
                    nameof(MoopelApiOptions), nameof(MoopelApiOptions.BaseUrl), options.BaseUrl, apiBaseUri);
            }

            client.BaseAddress = apiBaseUri;
        });

        _builder.Services.AddScoped<IAuthApiService, AuthApiService>();
        _builder.Services.AddScoped<IAuthService, AuthService>();
        _builder.Services.AddScoped<INotesService, NotesService>();

        return _builder;
    }

    public WebAssemblyHost BuildHost()
    {
        return _builder.Build();
    }
}
