using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;

namespace UnityReleaseNoteMCP.Tests;

// A new mock client for testing purposes
public class MockUnityReleaseClient : IUnityReleaseClient
{
    public List<UnityRelease>? ReleasesToReturn { get; set; }

    public Task<List<UnityRelease>> GetAllReleasesAsync(string? version = null, string? stream = null, CancellationToken cancellationToken = default)
    {
        if (ReleasesToReturn == null)
        {
            return Task.FromResult(new List<UnityRelease>());
        }

        IEnumerable<UnityRelease> query = ReleasesToReturn;

        if (!string.IsNullOrEmpty(version))
        {
            query = query.Where(r => r.Version.StartsWith(version));
        }
        if (!string.IsNullOrEmpty(stream))
        {
            // Note: The mock client only supports a single stream filter, similar to the real implementation's limitation.
            query = query.Where(r => r.Stream.Equals(stream, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(query.ToList());
    }

    public Task<string> GetPageContentAsync(string url, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Mocked Markdown Content");
    }
}


[TestFixture]
public class UnityReleaseToolTests
{
    private MockUnityReleaseClient _mockClient = null!;
    private UnityReleaseTool _tool = null!;
    private static readonly DateTime TestStartDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [SetUp]
    public void Setup()
    {
        _mockClient = new MockUnityReleaseClient();
        _tool = new UnityReleaseTool(_mockClient);
        _mockClient.ReleasesToReturn = CreateTestData();
    }

    private List<UnityRelease> CreateTestData()
    {
        var dummySize = new UnityReleaseDigitalValue { Unit = "BYTE", Value = 100 };
        return new List<UnityRelease>
        {
            new() // Index 0
            {
                Version = "2022.3.5f1", Stream = "LTS", ReleaseDate = TestStartDate.AddDays(-10),
                Downloads = new List<UnityReleaseDownload>
                {
                    new() { Platform = "WINDOWS", Architecture = "X86_64", DownloadSize = dummySize, InstalledSize = dummySize, Modules = new(), Type = "EXE", Url = ""},
                    new() { Platform = "MAC_OS", Architecture = "X86_64", DownloadSize = dummySize, InstalledSize = dummySize, Modules = new(), Type = "PKG", Url = ""},
                },
                ReleaseNotes = new UnityReleaseNotes { Url = "http://lts.url", Type = "MD" }, SkuFamily = "CLASSIC", UnityHubDeepLink = "unityhub://2022.3.5f1/1", ShortRevision = "1", ThirdPartyNotices = new()
            },
            new() // Index 1
            {
                Version = "2023.1.0a20", Stream = "ALPHA", ReleaseDate = TestStartDate.AddDays(-5),
                Downloads = new List<UnityReleaseDownload>
                {
                    new() { Platform = "WINDOWS", Architecture = "X86_64", DownloadSize = dummySize, InstalledSize = dummySize, Modules = new(), Type = "EXE", Url = ""},
                    new() { Platform = "LINUX", Architecture = "X86_64", DownloadSize = dummySize, InstalledSize = dummySize, Modules = new(), Type = "TAR_GZ", Url = ""},
                },
                ReleaseNotes = new UnityReleaseNotes { Url = "http://alpha.url", Type = "MD" }, SkuFamily = "CLASSIC", UnityHubDeepLink = "unityhub://2023.1.0a20/2", ShortRevision = "2", ThirdPartyNotices = new()
            },
            new() // Index 2
            {
                Version = "2023.2.0b1", Stream = "BETA", ReleaseDate = TestStartDate.AddDays(-3),
                Downloads = new List<UnityReleaseDownload>
                {
                    new() { Platform = "WINDOWS", Architecture = "ARM64", DownloadSize = dummySize, InstalledSize = dummySize, Modules = new(), Type = "EXE", Url = ""},
                },
                ReleaseNotes = new UnityReleaseNotes { Url = "http://beta.url", Type = "MD" }, SkuFamily = "CLASSIC", UnityHubDeepLink = "unityhub://2023.2.0b1/3", ShortRevision = "3", ThirdPartyNotices = new()
            },
            new() // Index 3
            {
                Version = "2022.3.10f1", Stream = "LTS", ReleaseDate = TestStartDate.AddDays(-1),
                Downloads = new List<UnityReleaseDownload>
                {
                    new() { Platform = "MAC_OS", Architecture = "ARM64", DownloadSize = dummySize, InstalledSize = dummySize, Modules = new(), Type = "PKG", Url = ""},
                },
                ReleaseNotes = new UnityReleaseNotes { Url = "http://lts-newer.url", Type = "MD" }, SkuFamily = "CLASSIC", UnityHubDeepLink = "unityhub://2022.3.10f1/4", ShortRevision = "4", ThirdPartyNotices = new()
            }
        };
    }

    [Test]
    public async Task GetUnityReleases_NoFilter_ReturnsAllReleases_DescOrderByDefault()
    {
        var result = await _tool.GetUnityReleases();
        Assert.That(result.Total, Is.EqualTo(4));
        Assert.That(result.Results, Has.Count.EqualTo(4));
        Assert.That(result.Results[0].Version, Is.EqualTo("2022.3.10f1")); // Newest
        Assert.That(result.Results[3].Version, Is.EqualTo("2022.3.5f1")); // Oldest
    }

    [Test]
    public async Task GetUnityReleases_OrderAsc_ReturnsReleasesInAscendingOrder()
    {
        var result = await _tool.GetUnityReleases(order: "RELEASE_DATE_ASC");
        Assert.That(result.Total, Is.EqualTo(4));
        Assert.That(result.Results, Has.Count.EqualTo(4));
        Assert.That(result.Results[0].Version, Is.EqualTo("2022.3.5f1")); // Oldest
        Assert.That(result.Results[3].Version, Is.EqualTo("2022.3.10f1")); // Newest
    }

    [Test]
    public async Task GetUnityReleases_FilterBySingleStream_ReturnsCorrectReleases()
    {
        var result = await _tool.GetUnityReleases(stream: new[] { "LTS" });
        Assert.That(result.Total, Is.EqualTo(2));
        Assert.That(result.Results.All(r => r.Stream == "LTS"), Is.True);
    }

    [Test]
    public async Task GetUnityReleases_FilterByMultipleStreams_ReturnsCorrectReleases()
    {
        var result = await _tool.GetUnityReleases(stream: new[] { "LTS", "BETA" });
        Assert.That(result.Total, Is.EqualTo(3));
        Assert.That(result.Results.All(r => r.Stream == "LTS" || r.Stream == "BETA"), Is.True);
    }

    [Test]
    public async Task GetUnityReleases_FilterByVersion_ReturnsCorrectReleases()
    {
        var result = await _tool.GetUnityReleases(version: "2022.3");
        Assert.That(result.Total, Is.EqualTo(2));
        Assert.That(result.Results.All(r => r.Version.StartsWith("2022.3")), Is.True);
    }

    [Test]
    public async Task GetUnityReleases_FilterByPlatform_ReturnsCorrectReleases()
    {
        var result = await _tool.GetUnityReleases(platform: new[] { "LINUX" });
        Assert.That(result.Total, Is.EqualTo(1));
        Assert.That(result.Results[0].Version, Is.EqualTo("2023.1.0a20"));
    }

    [Test]
    public async Task GetUnityReleases_FilterByMultiplePlatforms_ReturnsCorrectReleases()
    {
        var result = await _tool.GetUnityReleases(platform: new[] { "LINUX", "MAC_OS" });
        Assert.That(result.Total, Is.EqualTo(3)); // 2022.3.5f1 (MAC_OS), 2023.1.0a20 (LINUX), 2022.3.10f1 (MAC_OS)
    }

    [Test]
    public async Task GetUnityReleases_FilterByArchitecture_ReturnsCorrectReleases()
    {
        var result = await _tool.GetUnityReleases(architecture: new[] { "ARM64" });
        Assert.That(result.Total, Is.EqualTo(2));
        Assert.That(result.Results.Any(r => r.Version == "2023.2.0b1"), Is.True);
        Assert.That(result.Results.Any(r => r.Version == "2022.3.10f1"), Is.True);
    }

    [Test]
    public async Task GetUnityReleases_CombinedFilters_ReturnsCorrectReleases()
    {
        var result = await _tool.GetUnityReleases(stream: new[] {"LTS"}, platform: new[] {"WINDOWS"});
        Assert.That(result.Total, Is.EqualTo(1));
        Assert.That(result.Results[0].Version, Is.EqualTo("2022.3.5f1"));
    }

    [Test]
    public async Task GetUnityReleases_Pagination_LimitWorks()
    {
        var result = await _tool.GetUnityReleases(limit: 2);
        Assert.That(result.Total, Is.EqualTo(4));
        Assert.That(result.Results, Has.Count.EqualTo(2));
        Assert.That(result.Results[0].Version, Is.EqualTo("2022.3.10f1"));
        Assert.That(result.Results[1].Version, Is.EqualTo("2023.2.0b1"));
    }

    [Test]
    public async Task GetUnityReleases_Pagination_OffsetWorks()
    {
        var result = await _tool.GetUnityReleases(offset: 2, limit: 2);
        Assert.That(result.Total, Is.EqualTo(4));
        Assert.That(result.Results, Has.Count.EqualTo(2));
        Assert.That(result.Results[0].Version, Is.EqualTo("2023.1.0a20"));
        Assert.That(result.Results[1].Version, Is.EqualTo("2022.3.5f1"));
    }

    [Test]
    public async Task GetUnityReleases_NoResults_ReturnsEmptyConnection()
    {
        _mockClient.ReleasesToReturn = new List<UnityRelease>();
        var result = await _tool.GetUnityReleases();
        Assert.That(result.Total, Is.EqualTo(0));
        Assert.That(result.Results, Is.Empty);
    }
}
