namespace MoopelFrontend.Client.Api;

/// <summary>
/// Configuration for reaching MoopelBackend. Bound from the
/// <see cref="ConfigSections.MoopelApi"/> section of appsettings.
/// </summary>
public sealed class MoopelApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}
