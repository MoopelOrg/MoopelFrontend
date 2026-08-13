using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

using MoopelFrontend.Services;
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
    private readonly MoopelAppSettings _appSettings;

    public Startup(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);

        string env = _builder.Configuration["Environment"]
            ?? throw new("Could not find environment");
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
        _builder.Services.AddSingleton<MoopelAppSettings>(_appSettings);

        _builder.Services.AddOptions<MoopelApiOptions>()
            .Bind(_builder.Configuration.GetRequiredSection(nameof(MoopelApiOptions)))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                $"'{nameof(MoopelApiOptions)}:{nameof(MoopelApiOptions.BaseUrl)}' must be an absolute URI.")
            .ValidateOnStart();

        _builder.Services.AddAuthorizationCore();
        _builder.Services.AddCascadingAuthenticationState();
        _builder.Services.AddHttpContextAccessor();

        _builder.Services.AddScoped<ITokenStore, ServerTokenStoreService>();
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
