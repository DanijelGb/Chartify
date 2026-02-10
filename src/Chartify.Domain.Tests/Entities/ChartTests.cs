using Chartify.Domain.Entities;
using Xunit;

namespace Chartify.Domain.Tests.Entities;

public class ChartTests
{
    [Fact]
    public void Chart_CanBeCreatedWithDefaultValues()
    {
        // Act
        var chart = new Chart();

        // Assert
        Assert.NotNull(chart);
        Assert.Equal("", chart.Country);
        Assert.Empty(chart.Tracks);
    }

    [Fact]
    public void Chart_CanBeCreatedWithInitializers()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var tracks = new List<Track>
        {
            new Track { Id = "1", Name = "Song", Artist = "Artist", Rank = 1 }
        };

        // Act
        var chart = new Chart
        {
            Country = "US",
            Date = date,
            Tracks = tracks
        };

        // Assert
        Assert.Equal("US", chart.Country);
        Assert.Equal(date, chart.Date);
        Assert.Single(chart.Tracks);
    }

    [Fact]
    public void Chart_TracksProperty_IsInitOnly()
    {
        // Arrange
        var chart = new Chart();
        var newTracks = new List<Track>
        {
            new Track { Id = "1", Name = "Song", Artist = "Artist", Rank = 1 }
        };

        // Act & Assert
        chart = new Chart { Tracks = newTracks };
        Assert.Single(chart.Tracks);
    }

    [Fact]
    public void Track_CanBeCreatedWithValues()
    {
        // Act
        var track = new Track
        {
            Id = "track-id",
            Name = "Track Name",
            Artist = "Artist Name",
            Rank = 5
        };

        // Assert
        Assert.Equal("track-id", track.Id);
        Assert.Equal("Track Name", track.Name);
        Assert.Equal("Artist Name", track.Artist);
        Assert.Equal(5, track.Rank);
    }

    [Fact]
    public void Track_HasDefaultEmptyStringForIdAndName()
    {
        // Act
        var track = new Track();

        // Assert
        Assert.Equal("", track.Id);
        Assert.Equal("", track.Name);
        Assert.Equal("", track.Artist);
    }

    [Fact]
    public void Chart_CanContainMultipleTracks()
    {
        // Arrange
        var tracks = new List<Track>
        {
            new Track { Id = "1", Name = "First", Artist = "Artist 1", Rank = 1 },
            new Track { Id = "2", Name = "Second", Artist = "Artist 2", Rank = 2 },
            new Track { Id = "3", Name = "Third", Artist = "Artist 3", Rank = 3 }
        };

        // Act
        var chart = new Chart { Tracks = tracks };

        // Assert
        Assert.Equal(3, chart.Tracks.Count);
        Assert.Equal("First", chart.Tracks[0].Name);
        Assert.Equal("Second", chart.Tracks[1].Name);
        Assert.Equal("Third", chart.Tracks[2].Name);
    }
}
