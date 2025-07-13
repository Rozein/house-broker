// Updated Unit Tests for DeletePropertyCommandHandler with Attachment Deletion
// File: HouseBroker.Tests.Unit/Application/Property/Command/DeletePropertyCommandHandlerTests.cs

using FluentAssertions;
using HouseBroker.Application.Features.Property.Command.CreateProperty;
using HouseBroker.Application.Features.Property.Command.DeleteProperty;
using HouseBroker.Application.Interface;
using HouseBroker.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace HouseBroker.UnitTest.Application.Property;

public class DeletePropertyCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Domain.Entities.Property>> _mockPropertyRepository;
    private readonly Mock<IGenericRepository<PropertyAttachments>> _mockPropertyAttachmentRepository;
    private readonly DeletePropertyCommandHandler _handler;

    public DeletePropertyCommandHandlerTests()
    {
        _mockPropertyRepository = new Mock<IGenericRepository<Domain.Entities.Property>>();
        _mockPropertyAttachmentRepository = new Mock<IGenericRepository<PropertyAttachments>>();
        _handler = new DeletePropertyCommandHandler(
            _mockPropertyRepository.Object,
            _mockPropertyAttachmentRepository.Object,
            NullLogger<DeletePropertyCommandHandler>.Instance
        );
    }

    [Fact]
    public async Task Handle_ValidPropertyId_ShouldDeletePropertyAndAttachmentsSuccessfully()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new DeletePropertyCommand(propertyId);
        
        var existingProperty = new Domain.Entities.Property
        {
            Id = propertyId,
            Price = 500000
        };

        var attachments = new List<PropertyAttachments>
        {
            new PropertyAttachments { Id = Guid.NewGuid(), PropertyId = propertyId },
            new PropertyAttachments { Id = Guid.NewGuid(), PropertyId = propertyId }
        };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
            .ReturnsAsync(existingProperty);

        _mockPropertyRepository.Setup(r => r.DeleteAsync(existingProperty));

        // Mock the attachment query for ExecuteDeleteAsync
        var mockAttachmentQueryable = attachments.AsQueryable().BuildMockDbSet();
        _mockPropertyAttachmentRepository.Setup(r => r.GetAll())
            .Returns(mockAttachmentQueryable.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be("Property and its attachments deleted successfully.");

        _mockPropertyRepository.Verify(r => r.GetByIdAsync(propertyId), Times.Once);
        _mockPropertyRepository.Verify(r => r.DeleteAsync(existingProperty), Times.Once);
        _mockPropertyAttachmentRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task Handle_PropertyNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new DeletePropertyCommand(propertyId);

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
            .ReturnsAsync((Domain.Entities.Property?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();

        _mockPropertyRepository.Verify(r => r.GetByIdAsync(propertyId), Times.Once);
        _mockPropertyRepository.Verify(r => r.DeleteAsync(It.IsAny<Domain.Entities.Property>()), Times.Never);
        _mockPropertyAttachmentRepository.Verify(r => r.GetAll(), Times.Never);
    }

    [Fact]
    public async Task Handle_AttachmentDeletionThrowsException_ShouldPropagateException()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new DeletePropertyCommand(propertyId);
        
        var existingProperty = new Domain.Entities.Property { Id = propertyId };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId))
            .ReturnsAsync(existingProperty);

        _mockPropertyRepository.Setup(r => r.DeleteAsync(existingProperty));

        _mockPropertyAttachmentRepository.Setup(r => r.GetAll())
            .Throws(new InvalidOperationException("Attachment deletion failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Attachment deletion failed");
        
        // Verify property deletion was called but process failed at attachment deletion
        _mockPropertyRepository.Verify(r => r.DeleteAsync(existingProperty), Times.Once);
    }
}