using MoopelFrontend.Services;
using MoopelFrontend.Shared.Interfaces;
using MoopelFrontend.View;

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
        _builder = AddServices();
        _builder = AddLifetimeServices();

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
        _builder.Services.AddMoopelClientServices(_builder.Configuration);

        // Override token storage for server-side execution with a server implementation.
        _builder.Services.AddScoped<ITokenStore, ServerTokenStoreService>();

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

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(MoopelFrontend.Client._Imports).Assembly);
    }
}
