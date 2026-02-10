using Chartify.Api.Controllers;
using Chartify.Application.Interfaces;
using Chartify.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Chartify.Api.Tests.Controllers;

public class ChartsControllerTests
{
    private readonly Mock<IChartService> _mockChartService;
    private readonly Mock<ILogger<ChartsController>> _mockLogger;
    private readonly ChartsController _controller;

    public ChartsControllerTests()
    {
        _mockChartService = new Mock<IChartService>();
        _mockLogger = new Mock<ILogger<ChartsController>>();
        _controller = new ChartsController(_mockLogger.Object, _mockChartService.Object);
    }

    [Fact]
    public async Task GetCharts_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var expectedChart = new Chart
        {
            Country = "global",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tracks = new List<Track>
            {
                new Track { Id = "1", Name = "Top Song", Artist = "Top Artist", Rank = 1 },
                new Track { Id = "2", Name = "Second Song", Artist = "Second Artist", Rank = 2 }
            }
        };

        _mockChartService
            .Setup(s => s.GetGlobalTop100Async())
            .ReturnsAsync(expectedChart);

        // Act
        var result = await _controller.GetCharts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        
        var returnedChart = Assert.IsType<Chart>(okResult.Value);
        Assert.Equal("global", returnedChart.Country);
        Assert.Equal(2, returnedChart.Tracks.Count);
    }

    [Fact]
    public async Task GetCharts_WhenServiceThrowsException_Returns500StatusCode()
    {
        // Arrange
        _mockChartService
            .Setup(s => s.GetGlobalTop100Async())
            .ThrowsAsync(new InvalidOperationException("Chart not found"));

        // Act
        var result = await _controller.GetCharts();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetCharts_CallsChartServiceOnce()
    {
        // Arrange
        var chart = new Chart
        {
            Country = "global",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tracks = new List<Track>()
        };

        _mockChartService
            .Setup(s => s.GetGlobalTop100Async())
            .ReturnsAsync(chart);

        // Act
        await _controller.GetCharts();

        // Assert
        _mockChartService.Verify(s => s.GetGlobalTop100Async(), Times.Once);
    }

    [Fact]
    public async Task GetCharts_WithMultipleTracks_ReturnsAllTracks()
    {
        // Arrange
        var tracks = Enumerable.Range(1, 10)
            .Select(i => new Track 
            { 
                Id = i.ToString(), 
                Name = $"Song {i}", 
                Artist = $"Artist {i}", 
                Rank = i 
            })
            .ToList();

        var chart = new Chart
        {
            Country = "global",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tracks = tracks
        };

        _mockChartService
            .Setup(s => s.GetGlobalTop100Async())
            .ReturnsAsync(chart);

        // Act
        var result = await _controller.GetCharts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedChart = Assert.IsType<Chart>(okResult.Value);
        Assert.Equal(10, returnedChart.Tracks.Count);
    }

    [Fact]
    public async Task GetCharts_LogsInformationOnSuccess()
    {
        // Arrange
        var chart = new Chart
        {
            Country = "global",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tracks = new List<Track>()
        };

        _mockChartService
            .Setup(s => s.GetGlobalTop100Async())
            .ReturnsAsync(chart);

        // Act
        await _controller.GetCharts();

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching global top 100 chart")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCharts_LogsInformationOnException()
    {
        // Arrange
        _mockChartService
            .Setup(s => s.GetGlobalTop100Async())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await _controller.GetCharts();

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occured")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
