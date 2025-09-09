using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddHttpClient()
    .AddMemoryCache()
    .AddSingleton<IUnityReleaseClient, UnityReleaseClient>()
    .AddTransient<UnityReleaseTool>()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();