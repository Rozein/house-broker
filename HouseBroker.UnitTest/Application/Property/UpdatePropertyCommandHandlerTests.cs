using Xunit;
using Moq;
using FluentAssertions;
using HouseBroker.Application.Property.Command.UpdateProperty;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;

namespace HouseBroker.Tests.Unit.Application.Property.Command;

public class UpdatePropertyCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Domain.Entities.Property>> _mockPropertyRepository;
    private readonly Mock<ILocationService> _mockLocationService;
    private readonly UpdatePropertyCommandHandler _handler;

    public UpdatePropertyCommandHandlerTests()
    {
        _mockPropertyRepository = new Mock<IGenericRepository<Domain.Entities.Property>>();
        _mockLocationService = new Mock<ILocationService>();
        
        _handler = new UpdatePropertyCommandHandler(
            _mockPropertyRepository.Object,
            _mockLocationService.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdatePropertySuccessfully()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new UpdatePropertyCommand
        {
            Id = propertyId,
            Price = 600000,
            PropertyType = PropertyType.Land,
            LocationId = null,
            CityId = Guid.NewGuid(),
            Area = "Updated Area",
            PostalCode = "54321",
            IsSold = true,
            Feature = "Updated features"
        };

        var existingProperty = new Domain.Entities.Property
        {
            Id = propertyId,
            Price = 500000,
            PropertyType = PropertyType.House,
            LocationId = Guid.NewGuid(),
            IsSold = false,
            Feature = "Original features"
        };

        var newLocationId = Guid.NewGuid();

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
            .ReturnsAsync(existingProperty);

        _mockLocationService.Setup(s => s.GetOrCreateLocationIdAsync(
            command.LocationId,
            command.CityId,
            command.Area,
            command.PostalCode,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newLocationId);
        _mockPropertyRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Property>()));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(propertyId);

        _mockPropertyRepository.Verify(r => r.UpdateAsync(It.Is<Domain.Entities.Property>(p =>
            p.Price == command.Price &&
            p.PropertyType == command.PropertyType &&
            p.LocationId == newLocationId &&
            p.IsSold == command.IsSold &&
            p.Feature == command.Feature)), Times.Once);
    }

    [Fact]
    public async Task Handle_PropertyNotFound_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new UpdatePropertyCommand
        {
            Id = Guid.NewGuid(),
            Price = 600000,
            PropertyType = PropertyType.House,
            CityId = Guid.NewGuid()
        };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync((Domain.Entities.Property?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();

        _mockLocationService.Verify(s => s.GetOrCreateLocationIdAsync(
            It.IsAny<Guid?>(),
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _mockPropertyRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Property>()), Times.Never);
    }

    [Fact]
    public async Task Handle_LocationServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new UpdatePropertyCommand
        {
            Id = propertyId,
            Price = 600000,
            PropertyType = PropertyType.House,
            CityId = Guid.NewGuid()
        };

        var existingProperty = new Domain.Entities.Property { Id = propertyId };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
            .ReturnsAsync(existingProperty);

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
        _mockPropertyRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Property>()), Times.Never);
    }
}