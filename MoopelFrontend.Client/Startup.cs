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

    public async Task LoadRuntimeConfigurationAsync(CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri(_builder.HostEnvironment.BaseAddress, UriKind.Absolute)
        };

        await RuntimeConfigurationLoader.LoadAsync(
            _builder.Configuration,
            httpClient,
            cancellationToken);
    }

    private MoopelAppSettings CreateAppSettings()
    {
        string env = _builder.Configuration["Environment"]
            ?? throw new("Could not find environment");
        MoopelEnvironment environment = env.ToUpperInvariant() switch
        {
            "TEST" => MoopelEnvironment.Test,
            "DEVELOPMENT" => MoopelEnvironment.Test,
            "PRODUCTION" => MoopelEnvironment.Production,
            _ => throw new($"Invalid environment {env}")
        };

        return new()
        {
            Environment = environment
        };
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
        _builder.Services.AddSingleton(CreateAppSettings());

        _builder.Services.AddOptions<MoopelApiOptions>()
            .Bind(_builder.Configuration.GetRequiredSection(nameof(MoopelApiOptions)))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                $"'{nameof(MoopelApiOptions)}:{nameof(MoopelApiOptions.BaseUrl)}' must be an absolute URI.")
            .ValidateOnStart();

        _builder.Services.AddAuthorizationCore();
        _builder.Services.AddCascadingAuthenticationState();

        _builder.Services.AddScoped<ITokenStore, BrowserTokenStoreService>();
        _builder.Services.AddScoped<MoopelAuthStateProvider>();
        _builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MoopelAuthStateProvider>());

        _builder.Services.AddHttpClient<IMoopelApiService, MoopelApiService>((sp, client) =>
        {
            MoopelApiOptions options = sp.GetRequiredService<IOptions<MoopelApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
        });

        _builder.Services.AddScoped<IAuthService, AuthService>();
        _builder.Services.AddScoped<INotesService, NotesService>();

        return _builder;
    }

    public WebAssemblyHost BuildHost()
    {
        WebAssemblyHost app = _builder.Build();

        return app;
    }
}
