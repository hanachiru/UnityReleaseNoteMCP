using System.ComponentModel;
using System.Text.Json.Serialization;

namespace UnityReleaseNoteMCP.Domain;

[Description("A Unity Release Digital Value as defined on Release Live Platform.")]
public class UnityReleaseDigitalValue
{
    [JsonPropertyName("value")]
    [Description("The value of the Unity Release Digital Value.")]
    public required long Value { get; set; }

    [JsonPropertyName("unit")]
    [Description("The unit of the Unity Release Digital Value.enum: BYTE, KILOBYTE, MEGABYTE, GIGABYTE.")]
    public required string Unit { get; set; } = string.Empty;
}

[Description("A Unity Release Notes File as defined on Release Live Platform.")]
public class UnityReleaseNotes
{
    [JsonPropertyName("url")]
    [Description("The URL of the Unity Release notes.")]
    public required string Url { get; set; } = string.Empty;
    
    [JsonPropertyName("integrity")]
    [Description("The Subresource Integrity of the Unity Release notes as defined by the [W3C Recommendation Subresource Integrity](https://www.w3.org/TR/SRI/). For example, `sha1-OTVjZTI0ZTk5MDg0YTMyYTBmZTdiNTU1NTMwZGRhYjQ3OWMzYzc1MQo=`.")]
    public string Integrity { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    [Description("The file type of the Unity Release notes. enum: TEXT, TAR_GZ, TAR_XZ, ZIP, PKG, EXE, PO, DMG, LZMA, LZ4, MD, PDF")]
    public required string Type { get; set; } = string.Empty;
}

public class UnityReleaseDownload
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("architecture")]
    public string Architecture { get; set; } = string.Empty;

    [JsonPropertyName("downloadSize")]
    public UnityReleaseDigitalValue DownloadSize { get; set; } = new();

    [JsonPropertyName("installedSize")]
    public UnityReleaseDigitalValue InstalledSize { get; set; } = new();

    [JsonPropertyName("modules")]
    public List<object> Modules { get; set; } = new(); // Using object for now as module schema is complex
}

public class UnityRelease
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    [JsonPropertyName("releaseNotes")]
    public UnityReleaseNotes ReleaseNotes { get; set; } = new();

    [JsonPropertyName("stream")]
    public string Stream { get; set; } = string.Empty;

    [JsonPropertyName("downloads")]
    public List<UnityReleaseDownload> Downloads { get; set; } = new();

    [JsonPropertyName("recommended")]
    public bool Recommended { get; set; }
}

public class UnityReleaseOffsetConnection
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("results")]
    public List<UnityRelease> Results { get; set; } = new();
}
