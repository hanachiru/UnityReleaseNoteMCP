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
        _mockClient = new MockUnityReleaseClient();
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
        Assert.That(result, Does.Contain("--- Unity Official Releases ---"));
        Assert.That(result, Does.Contain("- 2022.3.5f1"));
        Assert.That(result, Does.Contain("- 2022.3.10f1"));
        Assert.That(result, Does.Not.Contain("b1"));
    }

    [Test]
    public async Task GetUnityReleases_Beta_ReturnsBetaReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var result = await _tool.GetUnityReleases("beta");

        // Assert
        Assert.That(result, Does.Contain("--- Unity Beta Releases ---"));
        Assert.That(result, Does.Contain("- 2023.1.0a20"));
        Assert.That(result, Does.Contain("- 2023.2.0b1"));
        Assert.That(result, Does.Not.Contain("f1"));
    }

    [Test]
    public async Task GetUnityReleases_ApiFailure_ReturnsErrorMessage()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act
        var result = await _tool.GetUnityReleases("official");

        // Assert
        Assert.That(result, Is.EqualTo("Failed to retrieve release data or no releases found."));
    }

    // --- Tests for GetLatestReleaseNotes ---

    [Test]
    public async Task GetLatestReleaseNotes_Official_FindsLatestAndReturnsSummary()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();
        _mockClient.PageContentToReturn = "<html><body><h1>Hello Unity 2022.3.10f1</h1></body></html>";

        // Act
        var result = await _tool.GetLatestReleaseNotes("official");

        // Assert
        Assert.That(result, Does.Contain("Latest official version: 2022.3.10f1"));
        Assert.That(result, Does.Contain("https://unity.com/releases/editor/whats-new/2022.3.10f1"));
        Assert.That(result, Does.Contain("Hello Unity"));
    }

    [Test]
    public async Task GetLatestReleaseNotes_Beta_FindsLatestAndReturnsSummary()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();
        _mockClient.PageContentToReturn = "<html><body><h1>Hello Unity 2023.2.0b1</h1></body></html>";

        // Act
        var result = await _tool.GetLatestReleaseNotes("beta");

        // Assert
        Assert.That(result, Does.Contain("Latest beta version: 2023.2.0b1"));
        Assert.That(result, Does.Contain("https://unity.com/releases/editor/whats-new/2023.2.0b1"));
        Assert.That(result, Does.Contain("Hello Unity"));
    }

    [Test]
    public async Task GetLatestReleaseNotes_ApiFailure_ReturnsErrorMessage()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act
        var result = await _tool.GetLatestReleaseNotes();

        // Assert
        Assert.That(result, Is.EqualTo("Failed to retrieve release data or no releases found."));
    }

    [Test]
    public async Task GetLatestReleaseNotes_NoMatchingReleases_ReturnsNotFoundMessage()
    {
        // Arrange
        _mockClient.ReleasesToReturn = new ApiReleasesResponse
        {
            Results = [new ReleaseInfo { Version = "2023.1.0a20" }] // Only beta
        };

        // Act
        var result = await _tool.GetLatestReleaseNotes("official");

        // Assert
        Assert.That(result, Is.EqualTo("No official releases found to determine the latest."));
    }
}
