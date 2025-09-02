using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Application;

public interface IUnityReleaseClient
{
    Task<ApiReleasesResponse?> GetReleasesAsync(CancellationToken cancellationToken = default);
    Task<string> GetPageContentAsync(string url, CancellationToken cancellationToken = default);
}
