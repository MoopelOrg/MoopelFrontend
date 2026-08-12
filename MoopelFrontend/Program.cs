using MoopelFrontend;

Startup startup = new(args);
startup.CreateBuilder();
await startup.BuildApp().RunAsync();
