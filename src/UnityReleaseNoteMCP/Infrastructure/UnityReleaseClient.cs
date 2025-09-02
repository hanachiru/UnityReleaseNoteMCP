using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Infrastructure;

public class UnityReleaseClient : IUnityReleaseClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string AllReleasesCacheKey = "AllReleases";
    private const string UnityReleaseApiUrl = "https://services.api.unity.com/unity/editor/release/v1/releases";

    public UnityReleaseClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("curl/8.5.0");
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
    }

    public async Task<ApiReleasesResponse?> GetReleasesAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(AllReleasesCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            var allReleases = new List<ReleaseInfo>();
            var offset = 0;
            const int limit = 25;
            int total;

            do
            {
                var url = $"{UnityReleaseApiUrl}?limit={limit}&offset={offset}";
                var response = await _httpClient.GetFromJsonAsync<ApiReleasesResponse>(url, cancellationToken);

                if (response?.Results == null) break;

                allReleases.AddRange(response.Results);
                total = response.Total;
                offset += limit;

            } while (allReleases.Count < total);

            return new ApiReleasesResponse
            {
                Results = allReleases,
                Total = allReleases.Count
            };
        });
    }

    public async Task<ReleaseInfo?> GetReleaseByVersionAsync(string version, CancellationToken cancellationToken = default)
    {
        var url = $"{UnityReleaseApiUrl}/{version}";
        try
        {
            var releaseInfo = await _httpClient.GetFromJsonAsync<ReleaseInfo>(url, cancellationToken);
            return releaseInfo;
        }
        catch (HttpRequestException)
        {
            return null; // Expected if not found
        }
    }

    public async Task<string> GetPageContentAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetStringAsync(url, cancellationToken);
        }
        catch (HttpRequestException)
        {
            return string.Empty;
        }
    }
}
