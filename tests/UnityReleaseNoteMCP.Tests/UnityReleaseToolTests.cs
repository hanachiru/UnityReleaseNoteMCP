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

    [SetUp]
    public void Setup()
    {
        _mockClient = new MockUnityReleaseClient();
        _tool = new UnityReleaseTool(_mockClient);
    }

    private List<UnityRelease> CreateTestData()
    {
        return
        [
            new()
            {
                Version = "2022.3.5f1", Stream = "LTS", ReleaseDate = DateTime.Now.AddDays(-10),
                ReleaseNotes = new UnityReleaseNotes { Url = "http://lts.url" }
            },
            new()
            {
                Version = "2023.1.0a20", Stream = "ALPHA", ReleaseDate = DateTime.Now.AddDays(-5),
                ReleaseNotes = new UnityReleaseNotes { Url = "http://alpha.url" }
            },
            new()
            {
                Version = "2023.2.0b1", Stream = "BETA", ReleaseDate = DateTime.Now.AddDays(-3),
                ReleaseNotes = new UnityReleaseNotes { Url = "http://beta.url" }
            },
            new()
            {
                Version = "2022.3.10f1", Stream = "LTS", ReleaseDate = DateTime.Now.AddDays(-1),
                ReleaseNotes = new UnityReleaseNotes { Url = "http://lts-newer.url" }
            }
        ];
    }

    [Test]
    public async Task GetReleases_NoFilter_ReturnsAllReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var result = await _tool.GetReleases();

        // Assert
        Assert.That(result, Has.Count.EqualTo(4));
    }

    [Test]
    public async Task GetReleases_FilterByStream_ReturnsCorrectReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var result = await _tool.GetReleases(stream: "LTS");

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(r => r.Stream == "LTS"), Is.True);
    }

    [Test]
    public async Task GetReleases_FilterByVersion_ReturnsCorrectReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var result = await _tool.GetReleases(version: "2022.3");

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(r => r.Version.StartsWith("2022.3")), Is.True);
    }

    [Test]
    public void GetReleases_NoResults_ThrowsException()
    {
        // Arrange
        _mockClient.ReleasesToReturn = new List<UnityRelease>();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetReleases());
        Assert.That(ex.Message, Is.EqualTo("No releases found matching the specified criteria."));
    }

    [Test]
    public async Task GetLatestLtsRelease_FindsCorrectRelease()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var release = await _tool.GetLatestLtsRelease();

        // Assert
        Assert.That(release, Is.Not.Null);
        Assert.That(release.Version, Is.EqualTo("2022.3.10f1"));
        Assert.That(release.Stream, Is.EqualTo("LTS"));
    }

    [Test]
    public void GetLatestLtsRelease_NoLtsReleases_ThrowsException()
    {
        // Arrange
        _mockClient.ReleasesToReturn = new List<UnityRelease>
        {
            new() { Version = "2023.1.0a20", Stream = "ALPHA" }
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetLatestLtsRelease());
        Assert.That(ex.Message, Is.EqualTo("Could not find the latest LTS release."));
    }
}
