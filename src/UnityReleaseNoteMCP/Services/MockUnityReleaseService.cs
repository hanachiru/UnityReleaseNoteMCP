using System.Text.Json;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Services;

public class MockUnityReleaseService : IUnityReleaseService
{
    private readonly IReadOnlyList<UnityRelease> _releases;

    public MockUnityReleaseService()
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(MockUnityReleaseService).Assembly.Location)!;
        var jsonPath = Path.Combine(assemblyLocation, "unity-releases.json");
        var json = File.ReadAllText(jsonPath);
        _releases = JsonSerializer.Deserialize<IReadOnlyList<UnityRelease>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<UnityRelease>();
    }

    public Task<UnityReleaseOffsetConnection> GetUnityReleasesAsync(
        int limit,
        int offset,
        string? order,
        IReadOnlyList<string>? stream,
        IReadOnlyList<string>? platform,
        IReadOnlyList<string>? architecture,
        string? version)
    {
        var query = _releases.AsQueryable();

        if (stream is { Count: > 0 })
        {
            query = query.Where(r => stream.Contains(r.Stream));
        }

        if (platform is { Count: > 0 })
        {
            query = query.Where(r => r.Downloads.Any(d => platform.Contains(d.Platform)));
        }

        if (architecture is { Count: > 0 })
        {
            query = query.Where(r => r.Downloads.Any(d => architecture.Contains(d.Architecture)));
        }

        if (!string.IsNullOrWhiteSpace(version))
        {
            query = query.Where(r => r.Version.Contains(version, StringComparison.OrdinalIgnoreCase));
        }

        query = order switch
        {
            "RELEASE_DATE_ASC" => query.OrderBy(r => r.ReleaseDate),
            _ => query.OrderByDescending(r => r.ReleaseDate)
        };

        var total = query.Count();
        var results = query.Skip(offset).Take(limit).ToList();

        var connection = new UnityReleaseOffsetConnection(offset, limit, total, results);

        return Task.FromResult(connection);
    }
}
