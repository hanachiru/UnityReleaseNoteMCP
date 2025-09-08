using UnityReleaseNoteMCP.Services;

namespace UnityReleaseNoteMCP.Tests;

public class MockUnityReleaseServiceTests
{
    private readonly MockUnityReleaseService _service = new();

    [Fact]
    public async Task GetUnityReleasesAsync_DefaultParameters_ReturnsFirstPageOrderedByDateDesc()
    {
        // Act
        var result = await _service.GetUnityReleasesAsync(10, 0, null, null, null, null, null);

        // Assert
        Assert.Equal(4, result.Total);
        Assert.Equal(10, result.Limit);
        Assert.Equal(0, result.Offset);
        Assert.Equal(4, result.Results.Count);
        Assert.Equal("2022.3.8f1", result.Results[0].Version); // Most recent
        Assert.Equal("2021.3.20f1", result.Results[3].Version); // Oldest in this dataset
    }

    [Fact]
    public async Task GetUnityReleasesAsync_WithLimitAndOffset_ReturnsCorrectPage()
    {
        // Act
        var result = await _service.GetUnityReleasesAsync(2, 1, null, null, null, null, null);

        // Assert
        Assert.Equal(4, result.Total);
        Assert.Equal(2, result.Limit);
        Assert.Equal(1, result.Offset);
        Assert.Equal(2, result.Results.Count);
        Assert.Equal("2023.1.0b5", result.Results[0].Version); // Second most recent
    }

    [Fact]
    public async Task GetUnityReleasesAsync_FilterByStream_ReturnsMatchingReleases()
    {
        // Act
        var result = await _service.GetUnityReleasesAsync(10, 0, null, new[] { "LTS" }, null, null, null);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.All(result.Results, r => Assert.Equal("LTS", r.Stream));
    }

    [Fact]
    public async Task GetUnityReleasesAsync_FilterByPlatform_ReturnsMatchingReleases()
    {
        // Act
        var result = await _service.GetUnityReleasesAsync(10, 0, null, null, new[] { "LINUX" }, null, null);

        // Assert
        Assert.Equal(1, result.Total);
        Assert.Equal("2023.2.0a3", result.Results[0].Version);
    }

    [Fact]
    public async Task GetUnityReleasesAsync_FilterByVersion_ReturnsMatchingReleases()
    {
        // Act
        var result = await _service.GetUnityReleasesAsync(10, 0, null, null, null, null, "2022.3");

        // Assert
        Assert.Equal(1, result.Total);
        Assert.Equal("2022.3.8f1", result.Results[0].Version);
    }

    [Fact]
    public async Task GetUnityReleasesAsync_OrderByDateAsc_ReturnsCorrectOrder()
    {
        // Act
        var result = await _service.GetUnityReleasesAsync(10, 0, "RELEASE_DATE_ASC", null, null, null, null);

        // Assert
        Assert.Equal(4, result.Total);
        Assert.Equal("2021.3.20f1", result.Results[0].Version); // Oldest
        Assert.Equal("2022.3.8f1", result.Results[3].Version); // Most recent
    }

    [Fact]
    public async Task GetUnityReleasesAsync_CombinedFilter_ReturnsCorrectReleases()
    {
        // Act
        var result = await _service.GetUnityReleasesAsync(10, 0, null, new[] { "LTS" }, new[] { "WINDOWS" }, null, "2022");

        // Assert
        Assert.Equal(1, result.Total);
        Assert.Equal("2022.3.8f1", result.Results[0].Version);
    }
}
