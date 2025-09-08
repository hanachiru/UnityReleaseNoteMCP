using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Application;

[McpServerToolType]
public class UnityReleaseTool
{
    private readonly IUnityReleaseClient _client;

    public UnityReleaseTool(IUnityReleaseClient unityReleaseClient)
    {
        _client = unityReleaseClient;
    }

    [McpServerTool(Name = "getUnityReleases"), Description("Get Unity Releases.")]
    public async Task<UnityReleaseOffsetConnection> GetUnityReleases(
        [Description("Limits the number of results returned per page (min 1, max 25).")] int limit = 10,
        [Description("Offsets the first n elements from the results.")] int offset = 0,
        [Description("Orders the returned results by Get Unity Releases Order.")] string? order = "RELEASE_DATE_DESC",
        [Description("Filters by Unity Release stream.")] IReadOnlyList<string>? stream = null,
        [Description("Filters by Unity Release download platform.")] IReadOnlyList<string>? platform = null,
        [Description("Filters by Unity Release download architecture.")] IReadOnlyList<string>? architecture = null,
        [Description("Filters by a full text search on the version string.")] string? version = null)
    {
        // The client only supports filtering by a single stream.
        // If we have one stream, pass it to the client for pre-filtering.
        // If we have more than one, we must fetch all versions and filter in memory.
        var clientStream = stream is { Count: 1 } ? stream[0] : null;
        var allReleases = await _client.GetAllReleasesAsync(version, clientStream);

        if (allReleases == null)
        {
            return new UnityReleaseOffsetConnection { Limit = limit, Offset = offset, Total = 0, Results = new List<UnityRelease>() };
        }

        // --- In-Memory Filtering ---
        var filteredReleases = allReleases.AsEnumerable();

        // Filter by stream (only if we didn't pre-filter via the client)
        if (clientStream == null && stream is { Count: > 0 })
        {
            var streamSet = new HashSet<string>(stream, StringComparer.OrdinalIgnoreCase);
            filteredReleases = filteredReleases.Where(r => streamSet.Contains(r.Stream));
        }

        // Filter by platform
        if (platform is { Count: > 0 })
        {
            var platformSet = new HashSet<string>(platform, StringComparer.OrdinalIgnoreCase);
            filteredReleases = filteredReleases.Where(r => r.Downloads.Any(d => platformSet.Contains(d.Platform)));
        }

        // Filter by architecture
        if (architecture is { Count: > 0 })
        {
            var architectureSet = new HashSet<string>(architecture, StringComparer.OrdinalIgnoreCase);
            filteredReleases = filteredReleases.Where(r => r.Downloads.Any(d => architectureSet.Contains(d.Architecture)));
        }

        // --- Ordering ---
        if ("RELEASE_DATE_ASC".Equals(order, StringComparison.OrdinalIgnoreCase))
        {
            filteredReleases = filteredReleases.OrderBy(r => r.ReleaseDate);
        }
        else
        {
            filteredReleases = filteredReleases.OrderByDescending(r => r.ReleaseDate);
        }

        var finalResults = filteredReleases.ToList();

        // --- Pagination ---
        var paginatedResults = finalResults.Skip(offset).Take(limit).ToList();

        return new UnityReleaseOffsetConnection
        {
            Offset = offset,
            Limit = limit,
            Total = finalResults.Count,
            Results = paginatedResults
        };
    }
}
