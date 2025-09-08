using Cocona;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;
using UnityReleaseNoteMCP.Infrastructure;

public class Cli
{
    public static void Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);

        // Set up DI
        builder.Services.AddHttpClient();
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IUnityReleaseClient, UnityReleaseClient>();
        builder.Services.AddSingleton<UnityReleaseTool>();

        var app = builder.Build();

        app.AddCommands<Commands>();
        app.Run();
    }
}

public class Commands
{
    [Command("releases", Description = "Gets a list of Unity Editor releases, with optional filters.")]
    public async Task GetReleases(
        [FromService] UnityReleaseTool tool,
        [Option('v', Description = "Filter by a full or partial version string (e.g., '2022.3', '2023.1.0a22')")] string? version = null,
        [Option('s', Description = "Filter by release stream ('LTS', 'BETA', 'ALPHA', 'TECH')")] string? stream = null)
    {
        try
        {
            var releases = await tool.GetReleases(version, stream);
            Console.WriteLine($"Found {releases.Count} release(s) matching criteria:");
            foreach (var r in releases)
            {
                Console.WriteLine($"- {r.Version} ({r.Stream}) released on {r.ReleaseDate:yyyy-MM-dd}");
            }
        }
        catch (ToolExecutionException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    [Command("latest-lts", Description = "Gets the full data for the latest official LTS release.")]
    public async Task GetLatestLtsRelease([FromService] UnityReleaseTool tool)
    {
        try
        {
            var release = await tool.GetLatestLtsRelease();
            Console.WriteLine($"Latest LTS Release: {release.Version}");
            Console.WriteLine($"  Release Date: {release.ReleaseDate:yyyy-MM-dd}");
            Console.WriteLine($"  Stream: {release.Stream}");
            Console.WriteLine($"  Release Notes URL: {release.ReleaseNotes.Url}");
        }
        catch (ToolExecutionException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
