using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Application;

public interface IUnityReleaseClient
{
    Task<ApiReleasesResponse?> GetReleasesAsync(CancellationToken cancellationToken = default);
    Task<ReleaseInfo?> GetReleaseByVersionAsync(string version, CancellationToken cancellationToken = default);
    Task<string> GetPageContentAsync(string url, CancellationToken cancellationToken = default);
}
