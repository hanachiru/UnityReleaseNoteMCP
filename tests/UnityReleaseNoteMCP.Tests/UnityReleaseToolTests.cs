using NUnit.Framework;
using System.Threading.Tasks;
using UnityReleaseNoteMCP.Application;
using UnityReleaseNoteMCP.Domain;
using UnityReleaseNoteMCP.Tests.Mocks;

namespace UnityReleaseNoteMCP.Tests;

[TestFixture]
public class UnityReleaseToolTests
{
    private MockUnityReleaseClient _mockClient = null!;
    private UnityReleaseTool _tool = null!;

    [SetUp]
    public void Setup()
    {
        _mockClient = new MockUnityReleaseClient
        {
            PageContentToReturn = "<html><body>Mocked release notes content.</body></html>"
        };
        _tool = new UnityReleaseTool(_mockClient);
    }

    private ApiReleasesResponse CreateTestData()
    {
        return new ApiReleasesResponse
        {
            Results =
            [
                new ReleaseInfo { Version = "2022.3.5f1" }, // Official
                new ReleaseInfo { Version = "2023.1.0a20" }, // Beta
                new ReleaseInfo { Version = "2023.2.0b1" }, // Beta, newer
                new ReleaseInfo { Version = "2022.3.10f1" } // Official, newer
            ]
        };
    }

    [Test]
    public async Task GetUnityReleases_Official_ReturnsOfficialReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var listResult = await _tool.GetUnityReleases("official");

        // Assert
        Assert.That(listResult.ReleaseType, Is.EqualTo("official"));
        Assert.That(listResult.Versions, Contains.Item("2022.3.5f1"));
        Assert.That(listResult.Versions, Contains.Item("2022.3.10f1"));
        Assert.That(listResult.Versions, Has.None.Contains("b1"));
    }

    [Test]
    public async Task GetUnityReleases_Beta_ReturnsBetaReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var listResult = await _tool.GetUnityReleases("beta");

        // Assert
        Assert.That(listResult.ReleaseType, Is.EqualTo("beta"));
        Assert.That(listResult.Versions, Contains.Item("2023.1.0a20"));
        Assert.That(listResult.Versions, Contains.Item("2023.2.0b1"));
        Assert.That(listResult.Versions, Has.None.EndsWith("f1"));
    }

    [Test]
    public void GetUnityReleases_ApiFailure_ThrowsException()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetUnityReleases("official"));
        Assert.That(ex.Message, Is.EqualTo("Failed to retrieve release data or no releases found."));
    }

    [Test]
    public async Task GetLatestReleaseNotes_Official_FindsLatestAndReturnsNotesResult()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var notesResult = await _tool.GetLatestReleaseNotes("official");

        // Assert
        Assert.That(notesResult.Version, Is.EqualTo("2022.3.10f1"));
        Assert.That(notesResult.Url, Is.EqualTo("https://unity.com/releases/editor/whats-new/2022.3.10f1"));
        Assert.That(notesResult.Summary, Does.StartWith("<html>"));
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
            Results = [new ReleaseInfo { Version = "2023.1.0a20" }] // Only beta
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ToolExecutionException>(async () => await _tool.GetLatestReleaseNotes("official"));
        Assert.That(ex.Message, Is.EqualTo("No official releases found to determine the latest."));
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
        Assert.That(notesResult.Url, Does.Contain(version));
        Assert.That(notesResult.Summary, Does.StartWith("<html>"));
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
