using System.Text.Json.Serialization;

namespace UnityReleaseNoteMCP.Domain;

public class ReleaseData
{
    [JsonPropertyName("official")]
    public List<ReleaseInfo> Official { get; set; } = new();

    [JsonPropertyName("beta")]
    public List<ReleaseInfo> Beta { get; set; } = new();
}

public class ReleaseInfo
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("lts")]
    public bool Lts { get; set; }

    [JsonPropertyName("downloadUrl")]
    public string DownloadUrl { get; set; } = string.Empty;

    [JsonPropertyName("downloadSize")]
    public long DownloadSize { get; set; }

    [JsonPropertyName("installedSize")]
    public long InstalledSize { get; set; }

    [JsonPropertyName("checksum")]
    public string Checksum { get; set; } = string.Empty;

    [JsonPropertyName("modules")]
    public List<Module> Modules { get; set; } = new();
}

public class Module
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("downloadUrl")]
    public string DownloadUrl { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("visible")]
    public bool? Visible { get; set; }

    [JsonPropertyName("selected")]
    public bool? Selected { get; set; }
}
