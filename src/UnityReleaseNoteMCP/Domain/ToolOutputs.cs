namespace UnityReleaseNoteMCP.Domain;

/// <summary>
/// Represents a generic error result from a tool method.
/// </summary>
public class ErrorResult
{
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Represents the successful result of fetching a list of releases.
/// </summary>
public class ReleaseListResult
{
    public string ReleaseType { get; set; } = string.Empty;
    public List<string> Versions { get; set; } = new();
}

/// <summary>
/// Represents the successful result of fetching release notes.
/// </summary>
public class ReleaseNotesResult
{
    public string Version { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ReleaseType { get; set; } = string.Empty; // To specify 'official', 'beta', or specific version
}
