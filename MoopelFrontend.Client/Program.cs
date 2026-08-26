using MoopelFrontend.Client;

try
{
    Startup startup = new(args);
    await startup.LoadRuntimeConfigurationAsync();
    startup.CreateBuilder();
    await startup.BuildHost().RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine($"Error in Client Startup: {ex.Message}");
}