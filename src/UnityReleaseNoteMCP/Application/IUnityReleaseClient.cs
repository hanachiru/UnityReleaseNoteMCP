using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Application;

public interface IUnityReleaseClient
{
    Task<List<UnityRelease>> GetAllReleasesAsync(string? version = null, string? stream = null, CancellationToken cancellationToken = default);
    Task<string> GetPageContentAsync(string url, CancellationToken cancellationToken = default);
}
