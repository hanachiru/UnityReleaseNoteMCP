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

    // NOTE: The user-specified URL (https://services.docs.unity.com/release/v1/) appears to be a
    // documentation page that could not be successfully resolved to a direct API endpoint in this environment.
    // This alternative URL, found during investigation, provides the necessary JSON data for Unity releases.
    private const string UnityReleaseDataUrl = "https://public-cdn.cloud.unity3d.com/hub/prod/releases-win32.json";

    public UnityReleaseClient(HttpClient httpClient, ILogger<UnityReleaseClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ReleaseData?> GetReleasesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var releaseData = await _httpClient.GetFromJsonAsync<ReleaseData>(UnityReleaseDataUrl, cancellationToken);
            return releaseData;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Error fetching release data from {Url}", UnityReleaseDataUrl);
            return null;
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Error deserializing release data from {Url}", UnityReleaseDataUrl);
            return null;
        }
    }
}
