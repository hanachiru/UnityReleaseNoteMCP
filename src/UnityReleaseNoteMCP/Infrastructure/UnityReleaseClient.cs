using System.Net.Http.Json;
using System.Text.Json;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;
using Microsoft.Extensions.Logging;

namespace UnityReleaseNoteMCP.Infrastructure;

public class UnityReleaseClient : IUnityReleaseClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UnityReleaseClient> _logger;

    private const string UnityReleaseApiUrl = "https://services.api.unity.com/unity/editor/release/v1/releases";

    public UnityReleaseClient(HttpClient httpClient, ILogger<UnityReleaseClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiReleasesResponse?> GetReleasesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // The API is paginated, for now we fetch the first 100 results which should be sufficient
            // to find the latest versions. A more robust implementation could handle pagination.
            var response = await _httpClient.GetFromJsonAsync<ApiReleasesResponse>(
                $"{UnityReleaseApiUrl}?limit=100",
                cancellationToken);
            return response;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Error fetching release data from {Url}", UnityReleaseApiUrl);
            return null;
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Error deserializing release data from {Url}", UnityReleaseApiUrl);
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
            _logger.LogError(e, "Error fetching page content from {Url}", url);
            return string.Empty; // Return empty string on failure
        }
    }
}
