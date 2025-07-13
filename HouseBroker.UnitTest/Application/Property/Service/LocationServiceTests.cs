using FluentAssertions;
using HouseBroker.Application.Features.Property.Command.CreateProperty;
using HouseBroker.Application.Interface;
using HouseBroker.Domain.Entities;
using HouseBroker.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace HouseBroker.UnitTest.Application.Property.Service;

public class LocationServiceTests
{
    private readonly Mock<IGenericRepository<Location>> _mockLocationRepository;
    private readonly LocationService _locationService;

    public LocationServiceTests()
    {
        _mockLocationRepository = new Mock<IGenericRepository<Location>>();
        _locationService = new LocationService(_mockLocationRepository.Object,NullLogger<LocationService>.Instance);
    }

    [Fact]
    public async Task GetOrCreateLocationIdAsync_WithValidLocationId_ShouldReturnLocationId()
    {
        // Arrange
        var existingLocationId = Guid.NewGuid();
        var cityId = Guid.NewGuid();

        // Act
        var result = await _locationService.GetOrCreateLocationIdAsync(
            existingLocationId, cityId, "Downtown", "12345", CancellationToken.None);

        // Assert
        result.Should().Be(existingLocationId);
        _mockLocationRepository.Verify(r => r.GetAllNoTracking(), Times.Never);
        _mockLocationRepository.Verify(r => r.InsertAsync(It.IsAny<Location>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateLocationIdAsync_WithExistingLocation_ShouldReturnExistingLocationId()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var area = "Downtown";
        var postalCode = "12345";
        var existingLocationId = Guid.NewGuid();

        var existingLocation = new Location
        {
            Id = existingLocationId,
            CityId = cityId,
            Area = area,
            PostalCode = postalCode
        };

        // Use MockQueryable.EntityFrameworkCore to properly mock EF queries
        var locations = new List<Location> { existingLocation };
        var mockQueryable = locations.AsQueryable().BuildMockDbSet();
        
        _mockLocationRepository.Setup(r => r.GetAllNoTracking()).Returns(mockQueryable.Object);

        // Act
        var result = await _locationService.GetOrCreateLocationIdAsync(
            null, cityId, area, postalCode, CancellationToken.None);

        // Assert
        result.Should().Be(existingLocationId);
        _mockLocationRepository.Verify(r => r.GetAllNoTracking(), Times.Once);
        _mockLocationRepository.Verify(r => r.InsertAsync(It.IsAny<Location>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateLocationIdAsync_WithNoExistingLocation_ShouldCreateNewLocation()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var area = "New Area";
        var postalCode = "99999";
        var newLocationId = Guid.NewGuid();

        // Mock empty result set
        var locations = new List<Location>();
        var mockQueryable = locations.AsQueryable().BuildMockDbSet();
        
        _mockLocationRepository.Setup(r => r.GetAllNoTracking()).Returns(mockQueryable.Object);
        _mockLocationRepository.Setup(r => r.InsertAsync(It.IsAny<Location>()))
            .Callback<Location>(l => l.Id = newLocationId)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _locationService.GetOrCreateLocationIdAsync(
            null, cityId, area, postalCode, CancellationToken.None);

        // Assert
        result.Should().Be(newLocationId);
        _mockLocationRepository.Verify(r => r.InsertAsync(It.Is<Location>(l =>
            l.CityId == cityId &&
            l.Area == area &&
            l.PostalCode == postalCode)), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateLocationIdAsync_WithPartialMatch_ShouldCreateNewLocation()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var area = "Downtown";
        var postalCode = "12345";
        var newLocationId = Guid.NewGuid();

        var locations = new List<Location>
        {
            // Same city and area, but different postal code
            new Location { Id = Guid.NewGuid(), CityId = cityId, Area = area, PostalCode = "54321" }
        };

        var mockQueryable = locations.AsQueryable().BuildMockDbSet();
        _mockLocationRepository.Setup(r => r.GetAllNoTracking()).Returns(mockQueryable.Object);
        _mockLocationRepository.Setup(r => r.InsertAsync(It.IsAny<Location>()))
            .Callback<Location>(l => l.Id = newLocationId)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _locationService.GetOrCreateLocationIdAsync(
            null, cityId, area, postalCode, CancellationToken.None);

        // Assert
        result.Should().Be(newLocationId);
        _mockLocationRepository.Verify(r => r.InsertAsync(It.IsAny<Location>()), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateLocationIdAsync_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        _mockLocationRepository.Setup(r => r.GetAllNoTracking())
            .Throws(new InvalidOperationException("Database connection error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _locationService.GetOrCreateLocationIdAsync(null, cityId, "Test", "12345", CancellationToken.None));

        exception.Message.Should().Be("Database connection error");
    }
}

