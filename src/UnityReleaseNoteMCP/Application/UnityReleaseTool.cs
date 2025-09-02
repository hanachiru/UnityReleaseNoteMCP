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
    public async Task<string> GetUnityReleases(string releaseType = "official")
    {
        var apiResponse = await _unityReleaseClient.GetReleasesAsync();
        if (apiResponse?.Results == null || !apiResponse.Results.Any())
        {
            return "Failed to retrieve release data or no releases found.";
        }

        var releases = releaseType.ToLower() switch
        {
            "beta" => apiResponse.Results.Where(r => !r.IsOfficial).ToList(),
            _ => apiResponse.Results.Where(r => r.IsOfficial).ToList(),
        };

        if (!releases.Any())
        {
            return $"No {releaseType} releases found.";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"--- Unity {char.ToUpper(releaseType[0]) + releaseType.Substring(1)} Releases ---");
        foreach (var release in releases)
        {
            sb.AppendLine($"- {release.Version}");
        }

        return sb.ToString();
    }

    [McpServerTool, Description("Gets the latest Unity Editor release notes. Specify 'official' or 'beta' as the release type.")]
    public async Task<string> GetLatestReleaseNotes(string releaseType = "official")
    {
        var apiResponse = await _unityReleaseClient.GetReleasesAsync();
        if (apiResponse?.Results == null || !apiResponse.Results.Any())
        {
            return "Failed to retrieve release data or no releases found.";
        }

        var releases = releaseType.ToLower() switch
        {
            "beta" => apiResponse.Results.Where(r => !r.IsOfficial).ToList(),
            _ => apiResponse.Results.Where(r => r.IsOfficial).ToList(),
        };

        if (!releases.Any())
        {
            return $"No {releaseType} releases found to determine the latest.";
        }

        var latestRelease = releases
            .Select(r => new { OriginalVersion = r.Version, ParsedVersion = ParseVersion(r.Version) })
            .Where(r => r.ParsedVersion != null)
            .OrderByDescending(r => r.ParsedVersion)
            .FirstOrDefault();

        if (latestRelease == null)
        {
            return "Could not determine the latest release version.";
        }

        var releaseNotesUrl = string.Format(ReleaseNotesUrlFormat, latestRelease.OriginalVersion);
        var pageContent = await _unityReleaseClient.GetPageContentAsync(releaseNotesUrl);

        if (string.IsNullOrWhiteSpace(pageContent))
        {
            return $"Found latest version {latestRelease.OriginalVersion}, but could not retrieve release notes from {releaseNotesUrl}";
        }

        // Basic summary: get first 500 chars, clean up whitespace.
        var summary = new string(pageContent.Take(500).ToArray()).Trim();

        return $"""
               Latest {releaseType} version: {latestRelease.OriginalVersion}
               Release Notes URL: {releaseNotesUrl}

               Release Notes Summary:
               {summary}...
               """;
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
}
