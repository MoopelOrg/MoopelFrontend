using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
        _builder = AddServices();
        _builder = AddLifetimeServices();

        return _builder;
    }

    public WebAssemblyHostBuilder AddServices()
    {
        return _builder;
    }

    public WebAssemblyHostBuilder AddLifetimeServices()
    {
        _builder.Services.AddMoopelClientServices(_builder.Configuration);

        return _builder;
    }

    public WebAssemblyHost BuildHost()
    {
        return _builder.Build();
    }
}
