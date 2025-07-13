using FluentAssertions;
using HouseBroker.Application.CurrentUserService;
using HouseBroker.Application.Features.Property.Command.CreateProperty;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace HouseBroker.UnitTest.Application.Property;

public class CreatePropertyCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Domain.Entities.Property>> _mockPropertyRepository;
    private readonly Mock<IGenericRepository<Location>> _mockLocationRepository;
    private readonly Mock<ILocationService> _mockLocationService;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IGenericRepository<Broker>> _mockBrokerRepository;
    private readonly CreatePropertyCommandHandler _handler;

    public CreatePropertyCommandHandlerTests()
    {
        _mockPropertyRepository = new Mock<IGenericRepository<Domain.Entities.Property>>();
        _mockLocationRepository = new Mock<IGenericRepository<Location>>();
        _mockLocationService = new Mock<ILocationService>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockBrokerRepository = new Mock<IGenericRepository<Broker>>();
        
        _handler = new CreatePropertyCommandHandler(
            _mockPropertyRepository.Object,
            _mockLocationRepository.Object,
            _mockLocationService.Object,
            _mockCurrentUserService.Object,
            _mockBrokerRepository.Object,
            NullLogger<CreatePropertyCommandHandler>.Instance
        );
    }

    private CreatePropertyCommand GetValidCreatePropertyCommand()
    {
        return new CreatePropertyCommand
        {
            Price = 500000,
            PropertyType = PropertyType.House,
            LocationId = null,
            CityId = Guid.NewGuid(),
            Area = "Downtown",
            PostalCode = "12345",
            Feature = "Beautiful house with garden"
        };
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreatePropertySuccessfully()
    {
        // Arrange
        var command = GetValidCreatePropertyCommand();
        var userId = Guid.NewGuid();
        var brokerId = Guid.NewGuid();
        var expectedLocationId = Guid.NewGuid();
        var expectedPropertyId = Guid.NewGuid();

        var brokers = new List<Broker>
        {
            new Broker { Id = brokerId, UserId = userId }
        };
        var mockBrokerQueryable = brokers.AsQueryable().BuildMockDbSet();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);

        _mockLocationService.Setup(s => s.GetOrCreateLocationIdAsync(
            command.LocationId,
            command.CityId,
            command.Area,
            command.PostalCode,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLocationId);

        _mockBrokerRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockBrokerQueryable.Object);

        _mockPropertyRepository.Setup(r => r.InsertAsync(It.IsAny<Domain.Entities.Property>()))
            .Callback<Domain.Entities.Property>(p => p.Id = expectedPropertyId)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(expectedPropertyId);

        // Verify location service was called
        _mockLocationService.Verify(s => s.GetOrCreateLocationIdAsync(
            command.LocationId,
            command.CityId,
            command.Area,
            command.PostalCode,
            It.IsAny<CancellationToken>()), Times.Once);

        // Verify broker lookup was performed
        _mockBrokerRepository.Verify(r => r.GetAllNoTracking(), Times.Once);

        // Verify property was inserted with correct data
        _mockPropertyRepository.Verify(r => r.InsertAsync(It.Is<Domain.Entities.Property>(p =>
            p.Price == command.Price &&
            p.PropertyType == command.PropertyType &&
            p.LocationId == expectedLocationId &&
            p.BrokerId == brokerId &&
            p.IsSold == false &&
            p.Feature == command.Feature)), Times.Once);
    }

    [Fact]
    public async Task Handle_BrokerNotFound_ShouldCreatePropertyWithNullBrokerId()
    {
        // Arrange
        var command = GetValidCreatePropertyCommand();
        var userId = Guid.NewGuid();
        var expectedLocationId = Guid.NewGuid();
        var expectedPropertyId = Guid.NewGuid();

        // Empty broker list (no broker found for current user)
        var brokers = new List<Broker>();
        var mockBrokerQueryable = brokers.AsQueryable().BuildMockDbSet();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);

        _mockLocationService.Setup(s => s.GetOrCreateLocationIdAsync(
            It.IsAny<Guid?>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLocationId);

        _mockBrokerRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockBrokerQueryable.Object);

        _mockPropertyRepository.Setup(r => r.InsertAsync(It.IsAny<Domain.Entities.Property>()))
            .Callback<Domain.Entities.Property>(p => p.Id = expectedPropertyId)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify property was created with null/default BrokerId
        _mockPropertyRepository.Verify(r => r.InsertAsync(It.Is<Domain.Entities.Property>(p =>
            p.BrokerId == Guid.Empty)), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingLocationId_ShouldUseExistingLocation()
    {
        // Arrange
        var existingLocationId = Guid.NewGuid();
        var command = new CreatePropertyCommand
        {
            Price = 750000,
            PropertyType = PropertyType.Apartment,
            LocationId = existingLocationId,
            CityId = Guid.NewGuid(),
            Area = "Uptown",
            PostalCode = "54321",
            Feature = "Modern apartment"
        };

        var userId = Guid.NewGuid();
        var brokerId = Guid.NewGuid();

        var brokers = new List<Broker>
        {
            new Broker { Id = brokerId, UserId = userId }
        };
        var mockBrokerQueryable = brokers.AsQueryable().BuildMockDbSet();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);

        _mockLocationService.Setup(s => s.GetOrCreateLocationIdAsync(
            existingLocationId,
            command.CityId,
            command.Area,
            command.PostalCode,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLocationId);

        _mockBrokerRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockBrokerQueryable.Object);

        _mockPropertyRepository.Setup(r => r.InsertAsync(It.IsAny<Domain.Entities.Property>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _mockLocationService.Verify(s => s.GetOrCreateLocationIdAsync(
            existingLocationId,
            command.CityId,
            command.Area,
            command.PostalCode,
            It.IsAny<CancellationToken>()), Times.Once);

        _mockPropertyRepository.Verify(r => r.InsertAsync(It.Is<Domain.Entities.Property>(p =>
            p.LocationId == existingLocationId)), Times.Once);
    }

    [Fact]
    public async Task Handle_LocationServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = GetValidCreatePropertyCommand();
        var userId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);

        _mockLocationService.Setup(s => s.GetOrCreateLocationIdAsync(
            It.IsAny<Guid?>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Location service error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Location service error");

        // Verify no further operations were attempted
        _mockBrokerRepository.Verify(r => r.GetAllNoTracking(), Times.Never);
        _mockPropertyRepository.Verify(r => r.InsertAsync(It.IsAny<Domain.Entities.Property>()), Times.Never);
    }

    [Fact]
    public async Task Handle_BrokerRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = GetValidCreatePropertyCommand();
        var userId = Guid.NewGuid();
        var locationId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);

        _mockLocationService.Setup(s => s.GetOrCreateLocationIdAsync(
            It.IsAny<Guid?>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(locationId);

        _mockBrokerRepository.Setup(r => r.GetAllNoTracking())
            .Throws(new InvalidOperationException("Broker lookup failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Broker lookup failed");

        // Verify location service was called but property insertion was not
        _mockLocationService.Verify(s => s.GetOrCreateLocationIdAsync(
            It.IsAny<Guid?>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockPropertyRepository.Verify(r => r.InsertAsync(It.IsAny<Domain.Entities.Property>()), Times.Never);
    }
}