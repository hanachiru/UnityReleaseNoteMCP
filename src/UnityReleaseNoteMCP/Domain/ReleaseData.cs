using System.Text.Json.Serialization;

namespace UnityReleaseNoteMCP.Domain;

public class ApiReleasesResponse
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("results")]
    public List<ReleaseInfo> Results { get; set; } = new();
}

public class ReleaseInfo
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    // The official API does not seem to provide LTS or release type info directly.
    // We can infer the type (e.g. 'f' for official, 'b' for beta, 'a' for alpha) from the version string.
    public bool IsOfficial => Version.Contains("f");
}
