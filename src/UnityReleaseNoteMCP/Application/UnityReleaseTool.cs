using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace UnityReleaseNoteMCP.Application;

/// <summary>
/// Fetches Unity Editor release information.
/// </summary>
[McpServerToolType]
public class UnityReleaseTool
{
    private readonly IUnityReleaseClient _unityReleaseClient;

    public UnityReleaseTool(IUnityReleaseClient unityReleaseClient)
    {
        _unityReleaseClient = unityReleaseClient;
    }

    [McpServerTool, Description("Gets available Unity Editor releases. Specify 'official', 'beta', or 'all' as the release type.")]
    public async Task<string> GetUnityReleases(string releaseType = "official")
    {
        var releaseData = await _unityReleaseClient.GetReleasesAsync();
        if (releaseData == null)
        {
            return "Failed to retrieve release data.";
        }

        var sb = new StringBuilder();
        bool foundAny = false;

        if (releaseType.Equals("official", StringComparison.OrdinalIgnoreCase) || releaseType.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            if (releaseData.Official.Any())
            {
                foundAny = true;
                sb.AppendLine("--- Unity Official Releases ---");
                foreach (var release in releaseData.Official)
                {
                    sb.AppendLine($"- {release.Version} {(release.Lts ? "(LTS)" : "")}");
                }
            }
        }

        if (releaseType.Equals("beta", StringComparison.OrdinalIgnoreCase) || releaseType.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            if (releaseData.Beta.Any())
            {
                foundAny = true;
                if (sb.Length > 0) sb.AppendLine();
                sb.AppendLine("--- Unity Beta Releases ---");
                foreach (var release in releaseData.Beta)
                {
                    sb.AppendLine($"- {release.Version}");
                }
            }
        }

        if (!foundAny)
        {
             return $"No releases found for type '{releaseType}'. Please use 'official', 'beta', or 'all'.";
        }

        return sb.ToString();
    }
}
