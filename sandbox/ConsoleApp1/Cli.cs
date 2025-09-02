using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Infrastructure;

// Using a class with a Main method as the entry point, since Program.cs is problematic.
public class Cli
{
    public static void Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);

        // Manually create instances
        var httpClient = new HttpClient();
        var releaseClient = new UnityReleaseClient(httpClient);
        var releaseTool = new UnityReleaseTool(releaseClient);

        // Register the manually created instance as a singleton
        builder.Services.AddSingleton(releaseTool);

        var app = builder.Build();

        app.AddCommands<Commands>();
        app.Run();
    }
}

public class Commands
{
    [Command("releases", Description = "Gets a list of available Unity Editor releases.")]
    public async Task GetReleases([FromService] UnityReleaseTool tool, [Option('t', Description = "The type of release ('official' or 'beta')")] string type = "official")
    {
        var result = await tool.GetUnityReleases(type);
        switch (result)
        {
            case ReleaseListResult listResult:
                Console.WriteLine($"--- Unity {char.ToUpper(listResult.ReleaseType[0]) + listResult.ReleaseType.Substring(1)} Releases ---");
                foreach (var version in listResult.Versions)
                {
                    Console.WriteLine($"- {version}");
                }
                break;
            case ErrorResult error:
                Console.WriteLine($"Error: {error.Message}");
                break;
        }
    }

    [Command("latest-notes", Description = "Gets the latest Unity Editor release notes.")]
    public async Task GetLatestNotes([FromService] UnityReleaseTool tool, [Option('t', Description = "The type of release ('official' or 'beta')")] string type = "official")
    {
        var result = await tool.GetLatestReleaseNotes(type);
        switch (result)
        {
            case ReleaseNotesResult notesResult:
                Console.WriteLine($"Latest {notesResult.ReleaseType} version: {notesResult.Version}");
                Console.WriteLine($"Release Notes URL: {notesResult.Url}");
                Console.WriteLine("\nRelease Notes Summary:");
                Console.WriteLine(notesResult.Summary);
                break;
            case ErrorResult error:
                Console.WriteLine($"Error: {error.Message}");
                break;
        }
    }

    [Command("notes-by-version", Description = "Gets the release notes for a specific Unity Editor version.")]
    public async Task GetNotesByVersion([FromService] UnityReleaseTool tool, [Argument(Description = "The full version string (e.g., '2022.3.8f1')")] string version)
    {
        var result = await tool.GetReleaseNotesByVersion(version);
        switch (result)
        {
            case ReleaseNotesResult notesResult:
                Console.WriteLine($"Release Notes for {notesResult.Version}:");
                Console.WriteLine($"URL: {notesResult.Url}");
                Console.WriteLine("\nSummary:");
                Console.WriteLine(notesResult.Summary);
                break;
            case ErrorResult error:
                Console.WriteLine($"Error: {error.Message}");
                break;
        }
    }
}
