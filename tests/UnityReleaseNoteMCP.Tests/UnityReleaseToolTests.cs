using NUnit.Framework;
using System.Threading.Tasks;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;
using UnityReleaseNoteMCP.Tests.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace UnityReleaseNoteMCP.Tests;

[TestFixture]
public class UnityReleaseToolTests
{
    private MockUnityReleaseClient _mockClient = null!;
    private IHtmlParser _mockHtmlParser = null!;
    private UnityReleaseTool _tool = null!;

    [SetUp]
    public void Setup()
    {
        _mockClient = new MockUnityReleaseClient
        {
            PageContentToReturn = "<html><body><p>Mocked summary.</p></body></html>"
        };
        _mockHtmlParser = new MockHtmlParser();
        _tool = new UnityReleaseTool(_mockClient, _mockHtmlParser);
    }

    private ApiReleasesResponse CreateTestData()
    {
        return new ApiReleasesResponse
        {
            Results = new List<ReleaseInfo>
            {
                new() { Version = "2022.3.5f1" },
                new() { Version = "2023.1.0a20" },
                new() { Version = "2023.2.0b1" },
                new() { Version = "2022.3.10f1" }
            }
        };
    }

    [Test]
    public async Task GetUnityReleases_Official_ReturnsOfficialReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var listResult = await _tool.GetUnityReleases(ReleaseType.Official);

        // Assert
        Assert.That(listResult.ReleaseType, Is.EqualTo("Official"));
        Assert.That(listResult.Versions, Has.Count.EqualTo(2));
        Assert.That(listResult.Versions, Contains.Item("2022.3.10f1"));
    }

    [Test]
    public async Task GetUnityReleases_Beta_ReturnsBetaReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var listResult = await _tool.GetUnityReleases(ReleaseType.Beta);

        // Assert
        Assert.That(listResult.ReleaseType, Is.EqualTo("Beta"));
        Assert.That(listResult.Versions, Has.Count.EqualTo(2));
        Assert.That(listResult.Versions, Contains.Item("2023.2.0b1"));
    }

    [Test]
    public void GetUnityReleases_ApiFailure_ThrowsException()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetUnityReleases());
        Assert.That(ex.Message, Is.EqualTo("Failed to retrieve release data or no releases found."));
    }

    [Test]
    public async Task GetLatestReleaseNotes_Official_FindsLatestAndReturnsNotesResult()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var notesResult = await _tool.GetLatestReleaseNotes(ReleaseType.Official);

        // Assert
        Assert.That(notesResult.Version, Is.EqualTo("2022.3.10f1"));
        Assert.That(notesResult.Summary, Is.EqualTo("Mocked summary."));
    }

    [Test]
    public void GetLatestReleaseNotes_ApiFailure_ThrowsException()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetLatestReleaseNotes());
        Assert.That(ex.Message, Is.EqualTo("Failed to retrieve release data or no releases found."));
    }

    [Test]
    public void GetLatestReleaseNotes_NoMatchingReleases_ThrowsException()
    {
        // Arrange
        _mockClient.ReleasesToReturn = new ApiReleasesResponse
        {
            Results = new List<ReleaseInfo> { new() { Version = "2023.1.0a20" } } // Only beta
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetLatestReleaseNotes(ReleaseType.Official));
        Assert.That(ex.Message, Is.EqualTo("No Official releases found to determine the latest."));
    }

    [Test]
    public async Task GetReleaseNotesByVersion_WhenVersionExists_ReturnsNotesResult()
    {
        // Arrange
        var version = "2022.3.8f1";

        // Act
        var notesResult = await _tool.GetReleaseNotesByVersion(version);

        // Assert
        Assert.That(notesResult.Version, Is.EqualTo(version));
        Assert.That(notesResult.Summary, Is.EqualTo("Mocked summary."));
    }

    [Test]
    public void GetReleaseNotesByVersion_WhenVersionDoesNotExist_ThrowsException()
    {
        // Arrange
        var version = "non-existent-version";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetReleaseNotesByVersion(version));
        Assert.That(ex.Message, Does.Contain("Could not find release information"));
    }

    [Test]
    public async Task GetReleasesByStream_WhenStreamExists_ReturnsMatchingReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();
        var stream = "2022.3";

        // Act
        var result = await _tool.GetReleasesByStream(stream);

        // Assert
        Assert.That(result.Versions, Has.Count.EqualTo(2));
        Assert.That(result.Versions, Contains.Item("2022.3.5f1"));
        Assert.That(result.Versions, Contains.Item("2022.3.10f1"));
    }
}
