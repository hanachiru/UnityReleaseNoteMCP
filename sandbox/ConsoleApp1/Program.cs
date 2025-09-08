using System.Net.Http.Json;
using UnityReleaseNoteMCP.Domain;

if (args.Length == 0)
{
    Console.WriteLine("Usage: ConsoleApp1 <version_filter>");
    Console.WriteLine("Example: ConsoleApp1 2022.3");
    return;
}

var versionFilter = args[0];
var httpClient = new HttpClient();

try
{
    var url = $"http://localhost:5102/unity/editor/release/v1/releases?version={versionFilter}";
    Console.WriteLine($"--> Calling API: {url}");

    var response = await httpClient.GetFromJsonAsync<UnityReleaseOffsetConnection>(url);

    if (response is not null)
    {
        Console.WriteLine($"<-- Found {response.Total} release(s).");
        foreach (var release in response.Results)
        {
            Console.WriteLine($"  - {release.Version} ({release.ReleaseDate.ToShortDateString()})");
        }
    }
    else
    {
        Console.WriteLine("<-- No response or empty response from server.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"<-- An error occurred: {ex.Message}");
}
