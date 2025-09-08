using System.ComponentModel;
using System.Text.Json.Serialization;
using UnityReleaseNoteMCP.Infrastructure;

namespace UnityReleaseNoteMCP.Domain;

[JsonConverter(typeof(UnityReleaseDigitalValueConverter))]
[Description("A Unity Release Digital Value as defined on Release Live Platform.")]
public class UnityReleaseDigitalValue
{
    [JsonPropertyName("value")]
    [Description("The value of the Unity Release Digital Value.")]
    public double? Value { get; set; }

    [JsonPropertyName("unit")]
    [Description("The unit of the Unity Release Digital Value. Can be 'BYTE', 'KILOBYTE', 'MEGABYTE', or 'GIGABYTE'.")]
    public required string Unit { get; set; }
}

[Description("A Unity Release Notes File as defined on Release Live Platform.")]
public class UnityReleaseNotes
{
    [JsonPropertyName("url")]
    [Description("The URL of the Unity Release notes.")]
    public required string Url { get; set; }

    [JsonPropertyName("integrity")]
    [Description("The Subresource Integrity of the Unity Release notes.")]
    public string? Integrity { get; set; }

    [JsonPropertyName("type")]
    [Description("The file type of the Unity Release notes.")]
    public required string Type { get; set; }
}

[Description("A Unity Release Third Party Notice File as defined on Release Live Platform.")]
public class UnityReleaseThirdPartyNotice
{
    [JsonPropertyName("originalFileName")]
    [Description("The original file name of the Unity Release Third Party Notice.")]
    public required string OriginalFileName { get; set; }

    [JsonPropertyName("url")]
    [Description("The URL of the Unity Release Third Party Notice.")]
    public required string Url { get; set; }

    [JsonPropertyName("integrity")]
    [Description("The Subresource Integrity of the Unity Release Third Party Notice.")]
    public string? Integrity { get; set; }

    [JsonPropertyName("type")]
    [Description("The file type of the Unity Release Third Party Notice.")]
    public required string Type { get; set; }
}

[Description("A Unity Release module end-user license agreement.")]
public class UnityReleaseModuleEula
{
    [JsonPropertyName("url")]
    [Description("The URL of the Unity Release module end-user license agreement.")]
    public required string Url { get; set; }

    [JsonPropertyName("integrity")]
    [Description("The Subresource Integrity of the Unity Release module end-user license agreement.")]
    public string? Integrity { get; set; }

    [JsonPropertyName("type")]
    [Description("The file type of the Unity Release module end-user license agreement.")]
    public required string Type { get; set; }

    [JsonPropertyName("label")]
    [Description("The label of the Unity Release module end-user license agreement.")]
    public required string Label { get; set; }

    [JsonPropertyName("message")]
    [Description("The message of the Unity Release module end-user license agreement.")]
    public required string Message { get; set; }
}

[Description("A Unity Release Module Extracted Path Rename.")]
public class UnityReleaseModuleExtractedPathRename
{
    [JsonPropertyName("from")]
    [Description("The location of the module when extracted.")]
    public required string From { get; set; }

    [JsonPropertyName("to")]
    [Description("The location the module must be moved to.")]
    public required string To { get; set; }
}

[Description("A Unity Release module.")]
public class UnityReleaseModule
{
    [JsonPropertyName("id")]
    [Description("The ID of the Unity Release module.")]
    public required string Id { get; set; }

    [JsonPropertyName("slug")]
    [Description("The slug of the Unity Release module. This is unique across all Unity Release Modules.")]
    public required string Slug { get; set; }

    [JsonPropertyName("name")]
    [Description("The name of the Unity Release module.")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    [Description("The description of the Unity Release module.")]
    public required string Description { get; set; }

    [JsonPropertyName("category")]
    [Description("The category of the Unity Release module.")]
    public required string Category { get; set; }

    [JsonPropertyName("url")]
    [Description("The URL of the Unity Release module.")]
    public required string Url { get; set; }

    [JsonPropertyName("integrity")]
    [Description("The Subresource Integrity of the Unity Release module.")]
    public string? Integrity { get; set; }

    [JsonPropertyName("type")]
    [Description("The file type of the Unity Release module.")]
    public required string Type { get; set; }

    [JsonPropertyName("downloadSize")]
    [Description("The download size of the Unity Release module.")]
    public UnityReleaseDigitalValue? DownloadSize { get; set; }

    [JsonPropertyName("installedSize")]
    [Description("The installed size of the Unity Release module.")]
    public UnityReleaseDigitalValue? InstalledSize { get; set; }

    [JsonPropertyName("subModules")]
    [Description("The optional sub-modules of the Unity Release module.")]
    public List<UnityReleaseModule>? SubModules { get; set; }

    [JsonPropertyName("required")]
    [Description("Indicator for whether the Unity Release module needs to be downloaded if the parent is downloaded.")]
    public bool Required { get; set; }

    [JsonPropertyName("hidden")]
    [Description("Indicator for whether the Unity Release module needs to be hidden when displaying modules in a UI.")]
    public bool Hidden { get; set; }

    [JsonPropertyName("extractedPathRename")]
    [Description("The location to move a Unity Release Module when extracted or unzipped.")]
    public UnityReleaseModuleExtractedPathRename? ExtractedPathRename { get; set; }

    [JsonPropertyName("preSelected")]
    [Description("Indicator for whether the Unity Release module should be pre-selected.")]
    public bool PreSelected { get; set; }

    [JsonPropertyName("destination")]
    [Description("The file destination of the Unity Release module.")]
    public string? Destination { get; set; }

    [JsonPropertyName("eula")]
    [Description("The end-user license agreements of the Unity Release module.")]
    public List<UnityReleaseModuleEula>? Eula { get; set; }
}

[Description("A Unity Release Hub download.")]
public class UnityReleaseDownload
{
    [JsonPropertyName("url")]
    [Description("The URL of the Unity Release Hub download.")]
    public required string Url { get; set; }

    [JsonPropertyName("integrity")]
    [Description("The Subresource Integrity of the Unity Release Hub download.")]
    public string? Integrity { get; set; }

    [JsonPropertyName("type")]
    [Description("The file type of the Unity Release Hub download.")]
    public required string Type { get; set; }

    [JsonPropertyName("platform")]
    [Description("The platform of the Unity Release Hub download.")]
    public required string Platform { get; set; }

    [JsonPropertyName("architecture")]
    [Description("The architecture of the Unity Release Hub download.")]
    public required string Architecture { get; set; }

    [JsonPropertyName("downloadSize")]
    [Description("The download size of the Unity Release Hub download.")]
    public required UnityReleaseDigitalValue DownloadSize { get; set; }

    [JsonPropertyName("installedSize")]
    [Description("The installed size of the Unity Release Hub download.")]
    public required UnityReleaseDigitalValue InstalledSize { get; set; }

    [JsonPropertyName("modules")]
    [Description("The modules of the Unity Release Hub download.")]
    public required List<UnityReleaseModule> Modules { get; set; }
}

[Description("A Unity Release.")]
public class UnityRelease
{
    [JsonPropertyName("version")]
    [Description("The version of the Unity Release.")]
    public required string Version { get; set; }

    [JsonPropertyName("releaseDate")]
    [Description("The release date of the Unity Release.")]
    public DateTime ReleaseDate { get; set; }

    [JsonPropertyName("releaseNotes")]
    [Description("The release notes of the Unity Release.")]
    public required UnityReleaseNotes ReleaseNotes { get; set; }

    [JsonPropertyName("stream")]
    [Description("The release stream of the Unity Release.")]
    public required string Stream { get; set; }

    [JsonPropertyName("downloads")]
    [Description("The downloads of the Unity Release.")]
    public required List<UnityReleaseDownload> Downloads { get; set; }

    [JsonPropertyName("skuFamily")]
    [Description("The SKU family of the Unity Release.")]
    public required string SkuFamily { get; set; }

    [JsonPropertyName("recommended")]
    [Description("Indicator for whether the Unity Release is the recommended LTS version.")]
    public bool Recommended { get; set; }

    [JsonPropertyName("unityHubDeepLink")]
    [Description("The Unity Hub deep link of the Unity Release.")]
    public required string UnityHubDeepLink { get; set; }

    [JsonPropertyName("shortRevision")]
    [Description("The Git Short Revision of the Unity Release.")]
    public required string ShortRevision { get; set; }

    [JsonPropertyName("thirdPartyNotices")]
    [Description("The Third Party Notices of the Unity Release.")]
    public required List<UnityReleaseThirdPartyNotice> ThirdPartyNotices { get; set; }
}

[Description("A relay style offset paginated Unity Release Connection.")]
public class UnityReleaseOffsetConnection
{
    [JsonPropertyName("offset")]
    [Description("The input offset of the Unity Release Offset Connection.")]
    public int Offset { get; set; }

    [JsonPropertyName("limit")]
    [Description("The input limit of the Unity Release Offset Connection.")]
    public int Limit { get; set; }

    [JsonPropertyName("total")]
    [Description("The total count of all available Unity Releases in the Unity Release Offset Connection.")]
    public int Total { get; set; }

    [JsonPropertyName("results")]
    [Description("The list of Unity Releases in the Unity Release Offset Connection.")]
    public required List<UnityRelease> Results { get; set; }
}
