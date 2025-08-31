using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Application;

public interface IUnityReleaseClient
{
    Task<ReleaseData?> GetReleasesAsync(CancellationToken cancellationToken = default);
}
