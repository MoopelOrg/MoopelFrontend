namespace MoopelFrontend.Client;

public static class RuntimeConfigurationLoader
{
    public static async Task LoadAsync(
        IConfigurationBuilder configuration,
        HttpClient httpClient,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(httpClient);

        using HttpResponseMessage response = await httpClient.GetAsync(
            "app-config.json",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        IConfigurationRoot runtimeConfiguration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        configuration.AddInMemoryCollection(runtimeConfiguration.AsEnumerable());
    }
}
