using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MoopelFrontend.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMoopelClientServices(builder.Configuration);

await builder.Build().RunAsync();
