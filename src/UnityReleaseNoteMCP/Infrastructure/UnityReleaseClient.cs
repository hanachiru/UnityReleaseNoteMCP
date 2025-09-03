using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Infrastructure;

public class UnityReleaseClient : IUnityReleaseClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string BaseUrl = "https://services.api.unity.com/unity/editor/release/v1/releases";

    public UnityReleaseClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<List<UnityRelease>> GetAllReleasesAsync(string? version = null, string? stream = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"AllReleases_{version}_{stream}";

        return (await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            var allReleases = new List<UnityRelease>();
            var offset = 0;
            const int limit = 25;
            int total;

            do
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["limit"] = limit.ToString();
                query["offset"] = offset.ToString();
                if (!string.IsNullOrEmpty(version)) query["version"] = version;
                if (!string.IsNullOrEmpty(stream)) query["stream"] = stream;

                var url = $"{BaseUrl}?{query}";

                var response = await _httpClient.GetFromJsonAsync<UnityReleaseOffsetConnection>(url, cancellationToken);

                if (response?.Results == null) break;

                allReleases.AddRange(response.Results);
                total = response.Total;
                offset += limit;

            } while (allReleases.Count < total);

            return allReleases;
        }))!;
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
