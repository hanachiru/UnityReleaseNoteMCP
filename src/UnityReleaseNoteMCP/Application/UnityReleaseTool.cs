using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Application;

/// <summary>
/// Fetches Unity Editor release information.
/// </summary>
[McpServerToolType]
public class UnityReleaseTool
{
    private readonly IUnityReleaseClient _unityReleaseClient;
    private const string ReleaseNotesUrlFormat = "https://unity.com/releases/editor/whats-new/{0}";

    public UnityReleaseTool(IUnityReleaseClient unityReleaseClient)
    {
        _unityReleaseClient = unityReleaseClient;
    }

    [McpServerTool, Description("Gets a list of available Unity Editor releases. Specify 'official' or 'beta' as the release type.")]
    public async Task<object> GetUnityReleases(string releaseType = "official")
    {
        var apiResponse = await _unityReleaseClient.GetReleasesAsync();
        if (apiResponse?.Results == null || !apiResponse.Results.Any())
        {
            return new ErrorResult { Message = "Failed to retrieve release data or no releases found." };
        }

        var releases = releaseType.ToLower() switch
        {
            "beta" => apiResponse.Results.Where(r => !r.IsOfficial).ToList(),
            _ => apiResponse.Results.Where(r => r.IsOfficial).ToList(),
        };

        if (!releases.Any())
        {
            return new ErrorResult { Message = $"No {releaseType} releases found." };
        }

        return new ReleaseListResult
        {
            ReleaseType = releaseType,
            Versions = releases.Select(r => r.Version).ToList()
        };
    }

    [McpServerTool, Description("Gets the latest Unity Editor release notes. Specify 'official' or 'beta' as the release type.")]
    public async Task<object> GetLatestReleaseNotes(string releaseType = "official")
    {
        var apiResponse = await _unityReleaseClient.GetReleasesAsync();
        if (apiResponse?.Results == null || !apiResponse.Results.Any())
        {
            return new ErrorResult { Message = "Failed to retrieve release data or no releases found." };
        }

        var releases = releaseType.ToLower() switch
        {
            "beta" => apiResponse.Results.Where(r => !r.IsOfficial).ToList(),
            _ => apiResponse.Results.Where(r => r.IsOfficial).ToList(),
        };

        if (!releases.Any())
        {
            return new ErrorResult { Message = $"No {releaseType} releases found to determine the latest." };
        }

        var latestRelease = releases
            .Select(r => new { OriginalVersion = r.Version, ParsedVersion = ParseVersion(r.Version) })
            .Where(r => r.ParsedVersion != null)
            .OrderByDescending(r => r.ParsedVersion)
            .FirstOrDefault();

        if (latestRelease == null)
        {
            return new ErrorResult { Message = "Could not determine the latest release version." };
        }

        var releaseNotesUrl = string.Format(ReleaseNotesUrlFormat, latestRelease.OriginalVersion);
        var pageContent = await _unityReleaseClient.GetPageContentAsync(releaseNotesUrl);

        if (string.IsNullOrWhiteSpace(pageContent))
        {
            return new ErrorResult { Message = $"Found latest version {latestRelease.OriginalVersion}, but could not retrieve release notes from {releaseNotesUrl}" };
        }

        var summary = new string(pageContent.Take(500).ToArray()).Trim();

        return new ReleaseNotesResult
        {
            ReleaseType = releaseType,
            Version = latestRelease.OriginalVersion,
            Url = releaseNotesUrl,
            Summary = $"{summary}..."
        };
    }

    private Version? ParseVersion(string versionString)
    {
        // System.Version can't parse suffixes like 'f1', 'b12', 'a1'.
        // We need to convert "2022.3.5f1" to "2022.3.5.1"
        var match = Regex.Match(versionString, @"(\d+\.\d+\.\d+)([abf])(\d+)");
        if (!match.Success)
        {
            return null;
        }

        var parsableVersionString = $"{match.Groups[1].Value}.{match.Groups[3].Value}";
        Version.TryParse(parsableVersionString, out var parsedVersion);
        return parsedVersion;
    }

    [McpServerTool, Description("Gets the release notes for a specific Unity Editor version.")]
    public async Task<object> GetReleaseNotesByVersion(string version)
    {
        // First, verify the version exists by calling the API.
        var releaseInfo = await _unityReleaseClient.GetReleaseByVersionAsync(version);
        if (releaseInfo == null)
        {
            return new ErrorResult { Message = $"Could not find release information for version '{version}'. It may not exist." };
        }

        var releaseNotesUrl = string.Format(ReleaseNotesUrlFormat, version);
        var pageContent = await _unityReleaseClient.GetPageContentAsync(releaseNotesUrl);

        if (string.IsNullOrWhiteSpace(pageContent))
        {
            return new ErrorResult { Message = $"Found version '{version}' but could not retrieve release notes from URL: {releaseNotesUrl}" };
        }

        var summary = new string(pageContent.Take(500).ToArray()).Trim();

        return new ReleaseNotesResult
        {
            ReleaseType = "specific version",
            Version = version,
            Url = releaseNotesUrl,
            Summary = $"{summary}..."
        };
    }
}
