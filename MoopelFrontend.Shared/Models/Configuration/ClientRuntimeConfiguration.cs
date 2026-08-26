namespace MoopelFrontend.Shared.Models.Configuration;

public sealed class ClientRuntimeConfiguration
{
    public required string Environment { get; init; }

    public required MoopelApiOptions MoopelApiOptions { get; init; }
}
