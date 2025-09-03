using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UnityReleaseNoteMCP.Domain;

public class UnityReleaseDigitalValue
{
    [JsonPropertyName("value")]
    public long Value { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;
}

public class UnityReleaseNotes
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
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
