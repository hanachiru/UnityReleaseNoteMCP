using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Tests.Mocks;

public class MockUnityReleaseClient : IUnityReleaseClient
{
    public ApiReleasesResponse? ReleasesToReturn { get; set; }
    public string PageContentToReturn { get; set; } = string.Empty;

    public Task<ApiReleasesResponse?> GetReleasesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ReleasesToReturn);
    }

    public Task<string> GetPageContentAsync(string url, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(PageContentToReturn);
    }
}
