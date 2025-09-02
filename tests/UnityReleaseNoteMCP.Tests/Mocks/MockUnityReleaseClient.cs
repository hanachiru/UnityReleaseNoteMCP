using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Tests.Mocks;

public class MockUnityReleaseClient : IUnityReleaseClient
{
    public ReleaseData? ReleasesToReturn { get; set; }

    public Task<ReleaseData?> GetReleasesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ReleasesToReturn);
    }
}
