using ModelContextProtocol.Server;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

    [McpServerTool, Description("Gets a list of Unity Editor releases, with optional filters.")]
    public async Task<List<UnityRelease>> GetReleases(
        [Description("Filter by a full or partial version string (e.g., '2022.3', '2023.1.0a22')")] string? version = null,
        [Description("Filter by release stream ('LTS', 'BETA', 'ALPHA', 'TECH')")] string? stream = null)
    {
        var releases = await _client.GetAllReleasesAsync(version, stream);
        if (releases == null || !releases.Any())
        {
            throw new ToolExecutionException("No releases found matching the specified criteria.");
        }
        return releases;
    }

    [McpServerTool, Description("Gets the release notes markdown URL for the latest official LTS release.")]
    public async Task<string> GetLatestLtsReleaseNotesUrl()
    {
        var ltsReleases = await _client.GetAllReleasesAsync(stream: "LTS");

        var latestLts = ltsReleases?
            .Where(r => r.Version.Contains("f")) // Ensure it's an official release
            .Select(r => new { Release = r, ParsedVersion = ParseVersion(r.Version) })
            .Where(r => r.ParsedVersion != null)
            .OrderByDescending(r => r.ParsedVersion)
            .FirstOrDefault();

        if (latestLts == null)
        {
            throw new ToolExecutionException("Could not find the latest LTS release.");
        }

        return latestLts.Release.ReleaseNotes.Url;
    }

    private Version? ParseVersion(string versionString)
    {
        var match = Regex.Match(versionString, @"(\d+\.\d+\.\d+)([abf])(\d+)");
        if (!match.Success) return null;
        var parsableVersionString = $"{match.Groups[1].Value}.{match.Groups[3].Value}";
        Version.TryParse(parsableVersionString, out var parsedVersion);
        return parsedVersion;
    }
}
