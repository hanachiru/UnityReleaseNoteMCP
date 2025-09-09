using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Infrastructure;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddHttpClient()
            .AddMemoryCache()
            .AddSingleton<IUnityReleaseClient, UnityReleaseClient>()
            .AddTransient<UnityReleaseTool>()
            .BuildServiceProvider();

        var unityTool = serviceProvider.GetService<UnityReleaseTool>();

        if (unityTool == null)
        {
            Console.WriteLine("Error: Could not create an instance of UnityReleaseTool.");
            return;
        }

        try
        {
            // Check if a version argument is provided
            if (args.Length > 0)
            {
                var version = args[0];
                Console.WriteLine($"Fetching release notes for version: {version}...");
                var notesContent = await unityTool.GetUnityReleaseNotesContent(version);

                Console.WriteLine("--- Release Notes Content ---");
                Console.WriteLine(notesContent);
                Console.WriteLine("-----------------------------");
            }
            else
            {
                Console.WriteLine("Fetching Unity releases (limit 5, stream LTS)...");
                var releases = await unityTool.GetUnityReleases(limit: 5, stream: new[] { "LTS" });

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                string jsonResult = JsonSerializer.Serialize(releases, options);

                Console.WriteLine("--- Results ---");
                Console.WriteLine(jsonResult);
                Console.WriteLine("---------------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Optional: Print stack trace for more details during development
            // Console.WriteLine(ex.ToString());
        }
    }
}
