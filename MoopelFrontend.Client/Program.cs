using MoopelFrontend.Client;

Startup startup = new(args);
startup.CreateBuilder();
await startup.BuildHost().RunAsync();
