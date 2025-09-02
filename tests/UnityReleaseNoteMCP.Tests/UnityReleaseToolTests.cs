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
        var result = await _tool.GetUnityReleases("official");

        // Assert
        Assert.That(result, Is.InstanceOf<ReleaseListResult>());
        var listResult = (ReleaseListResult)result;
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
        var result = await _tool.GetUnityReleases("beta");

        // Assert
        Assert.That(result, Is.InstanceOf<ReleaseListResult>());
        var listResult = (ReleaseListResult)result;
        Assert.That(listResult.ReleaseType, Is.EqualTo("beta"));
        Assert.That(listResult.Versions, Contains.Item("2023.1.0a20"));
        Assert.That(listResult.Versions, Contains.Item("2023.2.0b1"));
        Assert.That(listResult.Versions, Has.None.EndsWith("f1"));
    }

    [Test]
    public async Task GetUnityReleases_ApiFailure_ReturnsErrorResult()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act
        var result = await _tool.GetUnityReleases("official");

        // Assert
        Assert.That(result, Is.InstanceOf<ErrorResult>());
        var errorResult = (ErrorResult)result;
        Assert.That(errorResult.Message, Is.EqualTo("Failed to retrieve release data or no releases found."));
    }

    [Test]
    public async Task GetLatestReleaseNotes_Official_FindsLatestAndReturnsNotesResult()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var result = await _tool.GetLatestReleaseNotes("official");

        // Assert
        Assert.That(result, Is.InstanceOf<ReleaseNotesResult>());
        var notesResult = (ReleaseNotesResult)result;
        Assert.That(notesResult.Version, Is.EqualTo("2022.3.10f1"));
        Assert.That(notesResult.Url, Is.EqualTo("https://unity.com/releases/editor/whats-new/2022.3.10f1"));
        Assert.That(notesResult.Summary, Does.StartWith("<html>"));
    }

    [Test]
    public async Task GetLatestReleaseNotes_ApiFailure_ReturnsErrorResult()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act
        var result = await _tool.GetLatestReleaseNotes();

        // Assert
        Assert.That(result, Is.InstanceOf<ErrorResult>());
        var errorResult = (ErrorResult)result;
        Assert.That(errorResult.Message, Is.EqualTo("Failed to retrieve release data or no releases found."));
    }

    [Test]
    public async Task GetLatestReleaseNotes_NoMatchingReleases_ReturnsErrorResult()
    {
        // Arrange
        _mockClient.ReleasesToReturn = new ApiReleasesResponse
        {
            Results = [new ReleaseInfo { Version = "2023.1.0a20" }] // Only beta
        };

        // Act
        var result = await _tool.GetLatestReleaseNotes("official");

        // Assert
        Assert.That(result, Is.InstanceOf<ErrorResult>());
        var errorResult = (ErrorResult)result;
        Assert.That(errorResult.Message, Is.EqualTo("No official releases found to determine the latest."));
    }

    [Test]
    public async Task GetReleaseNotesByVersion_WhenVersionExists_ReturnsNotesResult()
    {
        // Arrange
        var version = "2022.3.8f1";

        // Act
        var result = await _tool.GetReleaseNotesByVersion(version);

        // Assert
        Assert.That(result, Is.InstanceOf<ReleaseNotesResult>());
        var notesResult = (ReleaseNotesResult)result;
        Assert.That(notesResult.Version, Is.EqualTo(version));
        Assert.That(notesResult.Url, Does.Contain(version));
        Assert.That(notesResult.Summary, Does.StartWith("<html>"));
    }

    [Test]
    public async Task GetReleaseNotesByVersion_WhenVersionDoesNotExist_ReturnsErrorResult()
    {
        // Arrange
        var version = "non-existent-version";

        // Act
        var result = await _tool.GetReleaseNotesByVersion(version);

        // Assert
        Assert.That(result, Is.InstanceOf<ErrorResult>());
        var errorResult = (ErrorResult)result;
        Assert.That(errorResult.Message, Does.Contain("Could not find release information"));
    }
}
