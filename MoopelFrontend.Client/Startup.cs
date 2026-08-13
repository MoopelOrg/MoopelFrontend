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
    private readonly MoopelAppSettings _appSettings;

    public Startup(string[] args)
    {
        _builder = WebAssemblyHostBuilder.CreateDefault(args);

        string? env = _builder.Configuration["Environment"];

        if (env is not null)
        {

            MoopelEnvironment Environment = env.ToUpper() switch
            {
                "TEST" => MoopelEnvironment.Test,
                "PRODUCTION" => MoopelEnvironment.Production,
                _ => throw new($"Invalid environment {env}")
            };
            _appSettings = new()
            {
                Environment = Environment
            };
        }
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
        _builder.Services.AddSingleton<MoopelAppSettings>(_appSettings);

        _builder.Services.AddOptions<MoopelApiOptions>()
            .Bind(_builder.Configuration.GetRequiredSection(nameof(MoopelApiOptions)))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                $"'{"MoopelApi"}:{nameof(MoopelApiOptions.BaseUrl)}' must be an absolute URI.")
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
        return _builder.Build();
    }
}
