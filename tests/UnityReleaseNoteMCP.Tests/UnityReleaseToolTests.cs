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

    private ReleaseData CreateTestData()
    {
        return new ReleaseData
        {
            Official = [new ReleaseInfo { Version = "2022.3.5f1", Lts = true }],
            Beta = [new ReleaseInfo { Version = "2023.2.0b1" }]
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
        Assert.That(result, Does.Contain("- 2022.3.5f1 (LTS)"));
        Assert.That(result, Does.Not.Contain("Beta"));
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
        Assert.That(result, Does.Contain("- 2023.2.0b1"));
        Assert.That(result, Does.Not.Contain("Official"));
    }

    [Test]
    public async Task GetUnityReleases_All_ReturnsAllReleases()
    {
        // Arrange
        _mockClient.ReleasesToReturn = CreateTestData();

        // Act
        var result = await _tool.GetUnityReleases("all");

        // Assert
        Assert.That(result, Does.Contain("--- Unity Official Releases ---"));
        Assert.That(result, Does.Contain("- 2022.3.5f1 (LTS)"));
        Assert.That(result, Does.Contain("--- Unity Beta Releases ---"));
        Assert.That(result, Does.Contain("- 2023.2.0b1"));
    }

    [Test]
    public async Task GetUnityReleases_ApiFailure_ReturnsErrorMessage()
    {
        // Arrange
        _mockClient.ReleasesToReturn = null;

        // Act
        var result = await _tool.GetUnityReleases("official");

        // Assert
        Assert.That(result, Is.EqualTo("Failed to retrieve release data."));
    }

    [Test]
    public async Task GetUnityReleases_NoMatchingReleases_ReturnsNotFoundMessage()
    {
        // Arrange
        _mockClient.ReleasesToReturn = new ReleaseData(); // Empty data

        // Act
        var result = await _tool.GetUnityReleases("official");

        // Assert
        Assert.That(result, Does.Contain("No releases found for type 'official'"));
    }
}
