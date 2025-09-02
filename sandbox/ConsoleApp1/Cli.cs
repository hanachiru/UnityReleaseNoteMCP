using Cocona;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;
using Microsoft.Extensions.Caching.Memory;
using UnityReleaseNoteMCP.Infrastructure;

public class Cli
{
    public static void Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);

        var httpClient = new HttpClient();
        var releaseClient = new UnityReleaseClient(httpClient);
        var releaseTool = new UnityReleaseTool(releaseClient);

        builder.Services.AddSingleton(releaseTool);

        var app = builder.Build();

        app.AddCommands<Commands>();
        app.Run();
    }
}

public class Commands
{
    [Command("releases", Description = "Gets a list of available Unity Editor releases.")]
    public async Task GetReleases([FromService] UnityReleaseTool tool, [Option('t', Description = "The type of release ('Official' or 'Beta')")] ReleaseType type = ReleaseType.Official)
    {
        try
        {
            var listResult = await tool.GetUnityReleases(type);
            Console.WriteLine($"--- Unity {listResult.ReleaseType} Releases ---");
            foreach (var version in listResult.Versions)
            {
                Console.WriteLine($"- {version}");
            }
        }
        catch (ToolExecutionException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    [Command("releases-by-stream", Description = "Gets a list of Unity releases for a specific stream (e.g., '2022.3').")]
    public async Task GetReleasesByStream([FromService] UnityReleaseTool tool, [Argument] string stream)
    {
        try
        {
            var listResult = await tool.GetReleasesByStream(stream);
            Console.WriteLine($"--- Unity Releases for Stream: {stream} ---");
            foreach (var version in listResult.Versions)
            {
                Console.WriteLine($"- {version}");
            }
        }
        catch (ToolExecutionException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    [Command("latest-notes", Description = "Gets the latest Unity Editor release notes.")]
    public async Task GetLatestNotes([FromService] UnityReleaseTool tool, [Option('t', Description = "The type of release ('Official' or 'Beta')")] ReleaseType type = ReleaseType.Official)
    {
        try
        {
            var notesResult = await tool.GetLatestReleaseNotes(type);
            Console.WriteLine($"Latest {notesResult.ReleaseType} version: {notesResult.Version}");
            Console.WriteLine($"Release Notes URL: {notesResult.Url}");
            Console.WriteLine("\nRelease Notes Summary:");
            Console.WriteLine(notesResult.Summary);
        }
        catch (ToolExecutionException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    [Command("notes-by-version", Description = "Gets the release notes for a specific Unity Editor version.")]
    public async Task GetNotesByVersion([FromService] UnityReleaseTool tool, [Argument(Description = "The full version string (e.g., '2022.3.8f1')")] string version)
    {
        try
        {
            var notesResult = await tool.GetReleaseNotesByVersion(version);
            Console.WriteLine($"Release Notes for {notesResult.Version}:");
            Console.WriteLine($"URL: {notesResult.Url}");
            Console.WriteLine("\nSummary:");
            Console.WriteLine(notesResult.Summary);
        }
        catch (ToolExecutionException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
