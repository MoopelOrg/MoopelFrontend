namespace MoopelFrontend.Shared.Models.Configuration;

/// <summary>
/// Configuration for reaching MoopelBackend.
/// </summary>
public sealed class MoopelApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}
