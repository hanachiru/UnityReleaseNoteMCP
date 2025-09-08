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
        // 1. Set up Dependency Injection
        var serviceProvider = new ServiceCollection()
            .AddHttpClient()
            .AddMemoryCache() // Add this line
            .AddSingleton<IUnityReleaseClient, UnityReleaseClient>()
            .AddTransient<UnityReleaseTool>()
            .BuildServiceProvider();

        // 2. Get an instance of the tool
        var unityTool = serviceProvider.GetService<UnityReleaseTool>();

        if (unityTool == null)
        {
            Console.WriteLine("Error: Could not create an instance of UnityReleaseTool.");
            return;
        }

        try
        {
            // 3. Call the method
            Console.WriteLine("Fetching Unity releases...");
            var releases = await unityTool.GetUnityReleases(limit: 5, stream: new[] { "LTS" });

            // 4. Serialize and print the result
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string jsonResult = JsonSerializer.Serialize(releases, options);

            Console.WriteLine("--- Results ---");
            Console.WriteLine(jsonResult);
            Console.WriteLine("---------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
