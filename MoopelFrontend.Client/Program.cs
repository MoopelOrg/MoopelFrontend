using MoopelFrontend.Client;

try
{
    Startup startup = new(args);
    startup.CreateBuilder();
    await startup.BuildHost().RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine($"Error in Client Startup: {ex.Message}");
}