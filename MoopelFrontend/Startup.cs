using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

using MoopelFrontend.Services;
using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Models.Configuration;
using MoopelFrontend.Shared.Services;
using MoopelFrontend.Shared.Services.Interfaces;
using MoopelFrontend.Shared.View;
using MoopelFrontend.View;

using Serilog;

namespace MoopelFrontend;

public sealed class Startup
{
    private WebApplicationBuilder _builder;

    public Startup(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
    }

    public WebApplicationBuilder CreateBuilder()
    {
        _builder = AddLogging();
        _builder = AddServices();
        _builder = AddLifetimeServices();

        return _builder;
    }

    public WebApplicationBuilder AddLogging()
    {
        _builder.Logging.ClearProviders();

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/moopel-frontend-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        _builder.Host.UseSerilog(Log.Logger);

        return _builder;
    }

    public WebApplicationBuilder AddServices()
    {
        _builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        return _builder;
    }

    public WebApplicationBuilder AddLifetimeServices()
    {
        _builder.Services.AddOptions<MoopelApiOptions>()
            .Bind(_builder.Configuration.GetSection(ConfigSections.MoopelApi));

        _builder.Services.AddAuthorizationCore();
        _builder.Services.AddCascadingAuthenticationState();

        _builder.Services.AddScoped<ITokenStore, ServerTokenStoreService>();
        _builder.Services.AddScoped<MoopelAuthStateProvider>();
        _builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MoopelAuthStateProvider>());

        _builder.Services.AddHttpClient<IMoopelApiService, MoopelApiService>((sp, client) =>
        {
            MoopelApiOptions options = sp.GetRequiredService<IOptions<MoopelApiOptions>>().Value;
            ILogger<Startup> logger = sp.GetRequiredService<ILogger<Startup>>();

            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out Uri? apiBaseUri))
            {
                string fallbackBaseUrl = _builder.Environment.IsDevelopment()
                    ? "https://localhost:7176/"
                    : "http://localhost/";
                apiBaseUri = new Uri(fallbackBaseUrl, UriKind.Absolute);

                logger.LogWarning("Invalid '{Section}:{Key}' value '{Value}'. Falling back to {FallbackBaseUrl}.",
                    ConfigSections.MoopelApi, nameof(MoopelApiOptions.BaseUrl), options.BaseUrl, fallbackBaseUrl);
            }

            client.BaseAddress = apiBaseUri;
        });

        _builder.Services.AddScoped<IAuthApiService, AuthApiService>();
        _builder.Services.AddScoped<IAuthService, AuthService>();
        _builder.Services.AddScoped<INotesService, NotesService>();

        return _builder;
    }

    public WebApplication BuildApp()
    {
        WebApplication app = _builder.Build();

        ConfigureGlobalExceptionLogging(app);
        ConfigurePipeline(app);

        return app;
    }

    private static void ConfigureGlobalExceptionLogging(WebApplication app)
    {
        ILogger<Startup> globalExceptionLogger = app.Services.GetRequiredService<ILogger<Startup>>();
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            globalExceptionLogger.LogCritical(e.ExceptionObject as Exception, "Unhandled AppDomain exception");
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            globalExceptionLogger.LogError(e.Exception, "Unobserved task exception");
            e.SetObserved();
        };
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseAntiforgery();

        app.MapPost("/api/clientlog", async (HttpContext context, ILogger<Startup> logger) =>
        {
            using StreamReader reader = new(context.Request.Body);
            string body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
                return Results.BadRequest();

            logger.LogInformation("[WASM] {Payload}", body);
            return Results.NoContent();
        });

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(MoopelFrontend.Client.Startup).Assembly);
    }
}
