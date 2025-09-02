using System.Net.Http.Json;
using System.Text.Json;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;
using Microsoft.Extensions.Logging;

namespace UnityReleaseNoteMCP.Infrastructure;

public class UnityReleaseClient : IUnityReleaseClient
{
    private readonly HttpClient _httpClient;

    private const string UnityReleaseApiUrl = "https://services.api.unity.com/unity/editor/release/v1/releases";

    public UnityReleaseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("curl/8.5.0");
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
    }

    public async Task<ApiReleasesResponse?> GetReleasesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{UnityReleaseApiUrl}?limit=25&offset=0";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ApiReleasesResponse>(jsonString, serializerOptions);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching release data from {UnityReleaseApiUrl}. Status Code: {e.StatusCode}. Message: {e.Message}");
            return null;
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Error deserializing release data from {UnityReleaseApiUrl}. Message: {e.Message}");
            return null;
        }
    }

    public async Task<ReleaseInfo?> GetReleaseByVersionAsync(string version, CancellationToken cancellationToken = default)
    {
        var url = $"{UnityReleaseApiUrl}/{version}";
        try
        {
            var releaseInfo = await _httpClient.GetFromJsonAsync<ReleaseInfo>(url, cancellationToken);
            return releaseInfo;
        }
        catch (HttpRequestException e)
        {
            // This is expected if the version doesn't exist (404 Not Found)
            Console.WriteLine($"Error fetching release data for version {version} from {url}. Status Code: {e.StatusCode}.");
            return null;
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Error deserializing release data for version {version} from {url}. Message: {e.Message}");
            return null;
        }
    }

    public async Task<string> GetPageContentAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetStringAsync(url, cancellationToken);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching page content from {url}. Message: {e.Message}");
            return string.Empty; // Return empty string on failure
        }
    }
}
