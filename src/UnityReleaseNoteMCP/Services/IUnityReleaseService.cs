using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Services;

public interface IUnityReleaseService
{
    Task<UnityReleaseOffsetConnection> GetUnityReleasesAsync(
        int limit,
        int offset,
        string? order,
        IReadOnlyList<string>? stream,
        IReadOnlyList<string>? platform,
        IReadOnlyList<string>? architecture,
        string? version);
}
