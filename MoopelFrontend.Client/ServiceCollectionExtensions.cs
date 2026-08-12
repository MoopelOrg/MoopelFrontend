using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

using MoopelFrontend.Client.Services;
using MoopelFrontend.Shared;
using MoopelFrontend.Shared.Interfaces;
using MoopelFrontend.Shared.Models.Configuration;
using MoopelFrontend.Shared.View;

namespace MoopelFrontend.Client;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the API client infrastructure and centralized authentication state.
    /// The backend base URL comes from the <see cref="ConfigSections.MoopelApi"/> configuration section.
    /// </summary>
    public static IServiceCollection AddMoopelClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<MoopelApiOptions>()
            .Bind(configuration.GetSection(ConfigSections.MoopelApi))
            .Validate(options => Uri.IsWellFormedUriString(options.BaseUrl, UriKind.Absolute),
                $"'{ConfigSections.MoopelApi}:{nameof(MoopelApiOptions.BaseUrl)}' must be an absolute URL.")
            .ValidateOnStart();

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        services.AddScoped<ITokenStore, BrowserTokenStoreService>();
        services.AddScoped<MoopelAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MoopelAuthStateProvider>());

        services.AddHttpClient<IMoopelApiService, MoopelApiService>((sp, client) =>
        {
            MoopelApiOptions options = sp.GetRequiredService<IOptions<MoopelApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
        });

        services.AddScoped<IAuthApiService, AuthApiService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INotesService, NotesService>();

        return services;
    }
}
