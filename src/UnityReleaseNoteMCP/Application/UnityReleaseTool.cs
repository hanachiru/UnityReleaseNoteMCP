using ModelContextProtocol.Server;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Application;

[McpServerToolType]
public class UnityReleaseTool
{
    private readonly IUnityReleaseClient _unityReleaseClient;
    private readonly IHtmlParser _htmlParser;
    private const string ReleaseNotesUrlFormat = "https://unity.com/releases/editor/whats-new/{0}";

    public UnityReleaseTool(IUnityReleaseClient unityReleaseClient, IHtmlParser htmlParser)
    {
        _unityReleaseClient = unityReleaseClient;
        _htmlParser = htmlParser;
    }

    private async Task<List<ReleaseInfo>> GetAndFilterReleases(ReleaseType releaseType)
    {
        var apiResponse = await _unityReleaseClient.GetReleasesAsync();
        if (apiResponse?.Results == null || !apiResponse.Results.Any())
        {
            throw new ToolExecutionException("Failed to retrieve release data or no releases found.");
        }

        return releaseType switch
        {
            ReleaseType.Beta => apiResponse.Results.Where(r => !r.IsOfficial).ToList(),
            _ => apiResponse.Results.Where(r => r.IsOfficial).ToList(),
        };
    }

    [McpServerTool, Description("Gets a list of available Unity Editor releases. Specify 'Official' or 'Beta' as the release type.")]
    public async Task<ReleaseListResult> GetUnityReleases(ReleaseType releaseType = ReleaseType.Official)
    {
        var releases = await GetAndFilterReleases(releaseType);
        if (!releases.Any())
        {
            throw new ToolExecutionException($"No {releaseType} releases found.");
        }

        return new ReleaseListResult
        {
            ReleaseType = releaseType.ToString(),
            Versions = releases.Select(r => r.Version).ToList()
        };
    }

    [McpServerTool, Description("Gets the latest Unity Editor release notes. Specify 'Official' or 'Beta' as the release type.")]
    public async Task<ReleaseNotesResult> GetLatestReleaseNotes(ReleaseType releaseType = ReleaseType.Official)
    {
        var releases = await GetAndFilterReleases(releaseType);
        if (!releases.Any())
        {
            throw new ToolExecutionException($"No {releaseType} releases found to determine the latest.");
        }

        var latestRelease = releases
            .Select(r => new { OriginalVersion = r.Version, ParsedVersion = ParseVersion(r.Version) })
            .Where(r => r.ParsedVersion != null)
            .OrderByDescending(r => r.ParsedVersion)
            .FirstOrDefault();

        if (latestRelease == null)
        {
            throw new ToolExecutionException("Could not determine the latest release version.");
        }

        var releaseNotesUrl = string.Format(ReleaseNotesUrlFormat, latestRelease.OriginalVersion);
        var pageContent = await _unityReleaseClient.GetPageContentAsync(releaseNotesUrl);

        if (string.IsNullOrWhiteSpace(pageContent))
        {
            throw new ToolExecutionException($"Found latest version {latestRelease.OriginalVersion}, but could not retrieve release notes from {releaseNotesUrl}");
        }

        var summary = _htmlParser.GetSummary(pageContent);

        return new ReleaseNotesResult
        {
            ReleaseType = releaseType.ToString(),
            Version = latestRelease.OriginalVersion,
            Url = releaseNotesUrl,
            Summary = summary
        };
    }

    private Version? ParseVersion(string versionString)
    {
        var match = Regex.Match(versionString, @"(\d+\.\d+\.\d+)([abf])(\d+)");
        if (!match.Success) return null;
        var parsableVersionString = $"{match.Groups[1].Value}.{match.Groups[3].Value}";
        Version.TryParse(parsableVersionString, out var parsedVersion);
        return parsedVersion;
    }

    [McpServerTool, Description("Gets the release notes for a specific Unity Editor version.")]
    public async Task<ReleaseNotesResult> GetReleaseNotesByVersion(string version)
    {
        var releaseInfo = await _unityReleaseClient.GetReleaseByVersionAsync(version);
        if (releaseInfo == null)
        {
            throw new ToolExecutionException($"Could not find release information for version '{version}'. It may not exist.");
        }

        var releaseNotesUrl = string.Format(ReleaseNotesUrlFormat, version);
        var pageContent = await _unityReleaseClient.GetPageContentAsync(releaseNotesUrl);

        if (string.IsNullOrWhiteSpace(pageContent))
        {
            throw new ToolExecutionException($"Found version '{version}' but could not retrieve release notes from URL: {releaseNotesUrl}");
        }

        var summary = _htmlParser.GetSummary(pageContent);

        return new ReleaseNotesResult
        {
            ReleaseType = "specific version",
            Version = version,
            Url = releaseNotesUrl,
            Summary = summary
        };
    }

    [McpServerTool, Description("Gets a list of Unity Editor releases for a specific major or minor version stream.")]
    public async Task<ReleaseListResult> GetReleasesByStream(string stream)
    {
        var apiResponse = await _unityReleaseClient.GetReleasesAsync();
        if (apiResponse?.Results == null || !apiResponse.Results.Any())
        {
            throw new ToolExecutionException("Failed to retrieve release data or no releases found.");
        }

        var releases = apiResponse.Results
            .Where(r => r.Version.StartsWith(stream))
            .Select(r => r.Version)
            .ToList();

        if (!releases.Any())
        {
            throw new ToolExecutionException($"No releases found for stream '{stream}'.");
        }

        return new ReleaseListResult
        {
            ReleaseType = $"Stream: {stream}",
            Versions = releases
        };
    }
}
