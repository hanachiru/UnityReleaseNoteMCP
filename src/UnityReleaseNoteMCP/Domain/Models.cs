using System.ComponentModel;
using System.Text.Json.Serialization;

namespace UnityReleaseNoteMCP.Domain;

[Description("A Unity Release Digital Value as defined on Release Live Platform.")]
public record UnityReleaseDigitalValue(
    [property: JsonPropertyName("value"), Description("The value of the Unity Release Digital Value.")] double Value,
    [property: JsonPropertyName("unit"), Description("The unit of the Unity Release Digital Value.")] string Unit
);

[Description("A Unity Release Notes File as defined on Release Live Platform.")]
public record UnityReleaseNotes(
    [property: JsonPropertyName("url"), Description("The URL of the Unity Release notes.")] string Url,
    [property: JsonPropertyName("integrity"), Description("The Subresource Integrity of the Unity Release notes as defined by the W3C Recommendation Subresource Integrity.")] string? Integrity,
    [property: JsonPropertyName("type"), Description("The file type of the Unity Release notes.")] string Type
);

[Description("A Unity Release Third Party Notice File as defined on Release Live Platform.")]
public record UnityReleaseThirdPartyNotice(
    [property: JsonPropertyName("originalFileName"), Description("The original file name of the Unity Release Third Party Notice.")] string OriginalFileName,
    [property: JsonPropertyName("url"), Description("The URL of the Unity Release Third Party Notice.")] string Url,
    [property: JsonPropertyName("integrity"), Description("The Subresource Integrity of the Unity Release Third Party Notice as defined by the W3C Recommendation Subresource Integrity.")] string? Integrity,
    [property: JsonPropertyName("type"), Description("The file type of the Unity Release Third Party Notice.")] string Type
);

[Description("A Unity Release module end-user license agreement.")]
public record UnityReleaseModuleEula(
    [property: JsonPropertyName("url"), Description("The URL of the Unity Release module end-user license agreement.")] string Url,
    [property: JsonPropertyName("integrity"), Description("The Subresource Integrity of the Unity Release module end-user license agreement as defined by the W3C Recommendation Subresource Integrity.")] string? Integrity,
    [property: JsonPropertyName("type"), Description("The file type of the Unity Release module end-user license agreement.")] string Type,
    [property: JsonPropertyName("label"), Description("The label of the Unity Release module end-user license agreement.")] string Label,
    [property: JsonPropertyName("message"), Description("The message of the Unity Release module end-user license agreement.")] string Message
);

[Description("A Unity Release Module Extracted Path Rename.")]
public record UnityReleaseModuleExtractedPathRename(
    [property: JsonPropertyName("from"), Description("The location of the module when extracted.")] string From,
    [property: JsonPropertyName("to"), Description("The location the module must be moved to.")] string To
);

[Description("A Unity Release module.")]
public record UnityReleaseModule(
    [property: JsonPropertyName("id"), Description("The ID of the Unity Release module.")] string Id,
    [property: JsonPropertyName("slug"), Description("The slug of the Unity Release module. This is unique across all Unity Release Modules.")] string Slug,
    [property: JsonPropertyName("name"), Description("The name of the Unity Release module.")] string Name,
    [property: JsonPropertyName("description"), Description("The description of the Unity Release module.")] string Description,
    [property: JsonPropertyName("category"), Description("The category of the Unity Release module.")] string Category,
    [property: JsonPropertyName("url"), Description("The URL of the Unity Release module.")] string Url,
    [property: JsonPropertyName("integrity"), Description("The Subresource Integrity of the Unity Release module as defined by the W3C Recommendation Subresource Integrity.")] string? Integrity,
    [property: JsonPropertyName("type"), Description("The file type of the Unity Release module.")] string Type,
    [property: JsonPropertyName("downloadSize"), Description("The download size of the Unity Release module.")] UnityReleaseDigitalValue DownloadSize,
    [property: JsonPropertyName("installedSize"), Description("The installed size of the Unity Release module.")] UnityReleaseDigitalValue InstalledSize,
    [property: JsonPropertyName("subModules"), Description("The optional sub-modules of the Unity Release module. The parent must be downloaded to download a sub-module.")] IReadOnlyList<UnityReleaseModule>? SubModules,
    [property: JsonPropertyName("required"), Description("The indicator for whether the Unity Release module needs to be downloaded if the parent is downloaded.")] bool Required,
    [property: JsonPropertyName("hidden"), Description("The indicator for whether the Unity Release module needs to be hidden when displaying modules in a UI.")] bool Hidden,
    [property: JsonPropertyName("extractedPathRename"), Description("The location to move a Unity Release Module when extracted or unzipped.")] UnityReleaseModuleExtractedPathRename? ExtractedPathRename,
    [property: JsonPropertyName("preSelected"), Description("The indicator for whether the Unity Release module should be pre-selected.")] bool PreSelected,
    [property: JsonPropertyName("destination"), Description("The file destination of the Unity Release module.")] string? Destination,
    [property: JsonPropertyName("eula"), Description("The end-user license agreements of the Unity Release module.")] IReadOnlyList<UnityReleaseModuleEula>? Eula
);

[Description("A Unity Release Hub download.")]
public record UnityReleaseDownload(
    [property: JsonPropertyName("url"), Description("The URL of the Unity Release Hub download.")] string Url,
    [property: JsonPropertyName("integrity"), Description("The Subresource Integrity of the Unity Release Hub download as defined by the W3C Recommendation Subresource Integrity.")] string? Integrity,
    [property: JsonPropertyName("type"), Description("The file type of the Unity Release Hub download.")] string Type,
    [property: JsonPropertyName("platform"), Description("The platform of the Unity Release Hub download.")] string Platform,
    [property: JsonPropertyName("architecture"), Description("The architecture of the Unity Release Hub download.")] string Architecture,
    [property: JsonPropertyName("downloadSize"), Description("The download size of the Unity Release Hub download.")] UnityReleaseDigitalValue DownloadSize,
    [property: JsonPropertyName("installedSize"), Description("The installed size of the Unity Release Hub download.")] UnityReleaseDigitalValue InstalledSize,
    [property: JsonPropertyName("modules"), Description("The modules of the Unity Release Hub download.")] IReadOnlyList<UnityReleaseModule> Modules
);

[Description("A Unity Release.")]
public record UnityRelease(
    [property: JsonPropertyName("version"), Description("The version of the Unity Release.")] string Version,
    [property: JsonPropertyName("releaseDate"), Description("The release date of the Unity Release.")] DateTime ReleaseDate,
    [property: JsonPropertyName("releaseNotes"), Description("The release notes of the Unity Release.")] UnityReleaseNotes ReleaseNotes,
    [property: JsonPropertyName("stream"), Description("The release stream of the Unity Release.")] string Stream,
    [property: JsonPropertyName("downloads"), Description("The downloads of the Unity Release.")] IReadOnlyList<UnityReleaseDownload> Downloads,
    [property: JsonPropertyName("skuFamily"), Description("The SKU family of the Unity Release.")] string SkuFamily,
    [property: JsonPropertyName("recommended"), Description("The indicator for whether the Unity Release is the recommended LTS version.")] bool Recommended,
    [property: JsonPropertyName("unityHubDeepLink"), Description("The Unity Hub deep link of the Unity Release.")] string UnityHubDeepLink,
    [property: JsonPropertyName("shortRevision"), Description("The Git Short Revision of the Unity Release.")] string ShortRevision,
    [property: JsonPropertyName("thirdPartyNotices"), Description("The Third Party Notices of the Unity Release.")] IReadOnlyList<UnityReleaseThirdPartyNotice> ThirdPartyNotices
);

[Description("A relay style offset paginated Unity Release Connection.")]
public record UnityReleaseOffsetConnection(
    [property: JsonPropertyName("offset"), Description("The input offset of the Unity Release Offset Connection.")] int Offset,
    [property: JsonPropertyName("limit"), Description("The input limit of the Unity Release Offset Connection.")] int Limit,
    [property: JsonPropertyName("total"), Description("The total count of all available Unity Releases in the Unity Release Offset Connection.")] int Total,
    [property: JsonPropertyName("results"), Description("The list of Unity Releases in the Unity Release Offset Connection.")] IReadOnlyList<UnityRelease> Results
);

// Error models
[Description("The Unity Release Bad Request Error.")]
public record GetUnityReleaseValidationError(
    [property: JsonPropertyName("title"), Description("The title of the Validation Error.")] string Title,
    [property: JsonPropertyName("status"), Description("The HTTP status code of the Validation Error.")] int Status,
    [property: JsonPropertyName("detail"), Description("The detail of the Validation Error.")] string Detail
);

[Description("The Unity Release Internal Server Error.")]
public record GetUnityReleaseInternalServerError(
    [property: JsonPropertyName("title"), Description("The title of the Internal Server Error.")] string Title,
    [property: JsonPropertyName("status"), Description("The HTTP status code of the Internal Server Error.")] int Status,
    [property: JsonPropertyName("detail"), Description("The detail of the Internal Server Error.")] string Detail
);

[Description("The Unity Release Service Unavailable Error.")]
public record GetUnityReleaseServiceUnavailableError(
    [property: JsonPropertyName("title"), Description("The title of the Service Unavailable Error.")] string Title,
    [property: JsonPropertyName("status"), Description("The HTTP status code of the Service Unavailable Error.")] int Status,
    [property: JsonPropertyName("detail"), Description("The detail of the Service Unavailable Error.")] string Detail
);
