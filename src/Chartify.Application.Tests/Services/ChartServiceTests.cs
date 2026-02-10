using Chartify.Application.Interfaces;
using Chartify.Application.Services;
using Chartify.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Chartify.Application.Tests.Services;

public class ChartServiceTests
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IChartRepository> _mockChartRepository;
    private readonly Mock<ILogger<ChartService>> _mockLogger;
    private readonly ChartService _chartService;

    public ChartServiceTests()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockChartRepository = new Mock<IChartRepository>();
        _mockLogger = new Mock<ILogger<ChartService>>();

        _chartService = new ChartService(
            _mockCacheService.Object,
            _mockChartRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetGlobalTop100Async_ShouldReturnChart()
    {
        // Arrange
        var expectedChart = new Chart
        {
            Country = "global",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tracks = new List<Track>
            {
                new Track { Id = "1", Name = "Song 1", Artist = "Artist 1", Rank = 1 }
            }
        };

        _mockCacheService
            .Setup(c => c.GetAsync<Chart>(It.IsAny<string>()))
            .ReturnsAsync((Chart?)null);

        _mockChartRepository
            .Setup(r => r.GetLatestAsync("global", It.IsAny<DateOnly>()))
            .ReturnsAsync(expectedChart);

        _mockCacheService
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<Chart>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _chartService.GetGlobalTop100Async();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("global", result.Country);
        Assert.Single(result.Tracks);
        Assert.Equal("Song 1", result.Tracks[0].Name);
        _mockChartRepository.Verify(r => r.GetLatestAsync("global", It.IsAny<DateOnly>()), Times.Once);
    }

    [Fact]
    public async Task GetChartAsync_WhenCachedExists_ShouldReturnCachedChart()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var cachedChart = new Chart
        {
            Country = "US",
            Date = date,
            Tracks = new List<Track>()
        };

        _mockCacheService
            .Setup(c => c.GetAsync<Chart>(It.IsAny<string>()))
            .ReturnsAsync(cachedChart);

        // Act
        var result = await _chartService.GetChartAsync("US", date);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("US", result.Country);
        _mockChartRepository.Verify(r => r.GetLatestAsync(It.IsAny<string>(), It.IsAny<DateOnly>()), Times.Never);
    }

    [Fact]
    public async Task GetChartAsync_WhenCacheIsEmpty_ShouldFetchFromRepository()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var repositoryChart = new Chart
        {
            Country = "UK",
            Date = date,
            Tracks = new List<Track> { new Track { Id = "1", Name = "Track", Artist = "Artist", Rank = 1 } }
        };

        _mockCacheService
            .Setup(c => c.GetAsync<Chart>(It.IsAny<string>()))
            .ReturnsAsync((Chart?)null);

        _mockChartRepository
            .Setup(r => r.GetLatestAsync("UK", date))
            .ReturnsAsync(repositoryChart);

        _mockCacheService
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<Chart>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _chartService.GetChartAsync("UK", date);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UK", result.Country);
        _mockChartRepository.Verify(r => r.GetLatestAsync("UK", date), Times.Once);
        _mockCacheService.Verify(c => c.SetAsync(It.IsAny<string>(), repositoryChart, TimeSpan.FromHours(24)), Times.Once);
    }

    [Fact]
    public async Task GetChartAsync_WhenRepositoryReturnsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        _mockCacheService
            .Setup(c => c.GetAsync<Chart>(It.IsAny<string>()))
            .ReturnsAsync((Chart?)null);

        _mockChartRepository
            .Setup(r => r.GetLatestAsync("NonExistent", date))
            .ReturnsAsync((Chart?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _chartService.GetChartAsync("NonExistent", date));

        Assert.Contains("No chart found", exception.Message);
    }

    [Fact]
    public async Task GetChartAsync_ShouldCacheChartWith24HourTTL()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var chart = new Chart
        {
            Country = "FR",
            Date = date,
            Tracks = new List<Track>()
        };

        _mockCacheService
            .Setup(c => c.GetAsync<Chart>(It.IsAny<string>()))
            .ReturnsAsync((Chart?)null);

        _mockChartRepository
            .Setup(r => r.GetLatestAsync("FR", date))
            .ReturnsAsync(chart);

        _mockCacheService
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<Chart>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        await _chartService.GetChartAsync("FR", date);

        // Assert
        _mockCacheService.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                chart,
                TimeSpan.FromHours(24)),
            Times.Once);
    }
}
