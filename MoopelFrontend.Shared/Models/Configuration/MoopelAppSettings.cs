namespace MoopelFrontend.Shared.Models.Configuration;

/// <summary>
/// General configuration for the application
/// </summary>
public class MoopelAppSettings
{
    public required MoopelEnvironment Environment { get; init; }
}

public enum MoopelEnvironment
{
    Test,
    Production
}
