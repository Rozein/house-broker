using FluentAssertions;
using HouseBroker.Application.Features.Property.Command.RemovePropertyAttachment;
using HouseBroker.Application.Interface;
using HouseBroker.Domain.Entities;
using Moq;
using Xunit;

namespace HouseBroker.UnitTest.Application.Property;
public class RemovePropertyAttachmentCommandHandlerTests
{
    private readonly Mock<IGenericRepository<PropertyAttachments>> _mockPropertyAttachmentRepository;
    private readonly RemovePropertyAttachmentCommandHandler _handler;

    public RemovePropertyAttachmentCommandHandlerTests()
    {
        _mockPropertyAttachmentRepository = new Mock<IGenericRepository<PropertyAttachments>>();
        _handler = new RemovePropertyAttachmentCommandHandler(_mockPropertyAttachmentRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidAttachmentId_ShouldDeleteAttachmentSuccessfully()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var command = new RemovePropertyAttachmentCommand(attachmentId);
        
        var existingAttachment = new PropertyAttachments
        {
            Id = attachmentId,
            PropertyId = Guid.NewGuid(),
            FileName = "test-image.jpg",
            ImageUrl = "uploads/property-images/test-image.jpg"
        };

        _mockPropertyAttachmentRepository.Setup(r => r.GetByIdAsync(attachmentId))
            .ReturnsAsync(existingAttachment);

        _mockPropertyAttachmentRepository.Setup(r => r.DeleteAsync(existingAttachment));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be("Attachment deleted successfully.");

        _mockPropertyAttachmentRepository.Verify(r => r.GetByIdAsync(attachmentId), Times.Once);
        _mockPropertyAttachmentRepository.Verify(r => r.DeleteAsync(existingAttachment), Times.Once);
    }

    [Fact]
    public async Task Handle_AttachmentNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var command = new RemovePropertyAttachmentCommand(attachmentId);

        _mockPropertyAttachmentRepository.Setup(r => r.GetByIdAsync(attachmentId))
            .ReturnsAsync((PropertyAttachments?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();

        _mockPropertyAttachmentRepository.Verify(r => r.GetByIdAsync(attachmentId), Times.Once);
        _mockPropertyAttachmentRepository.Verify(r => r.DeleteAsync(It.IsAny<PropertyAttachments>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var command = new RemovePropertyAttachmentCommand(attachmentId);

        _mockPropertyAttachmentRepository.Setup(r => r.GetByIdAsync(attachmentId))
            .ThrowsAsync(new InvalidOperationException("Database connection error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Database connection error");
        _mockPropertyAttachmentRepository.Verify(r => r.DeleteAsync(It.IsAny<PropertyAttachments>()), Times.Never);
    }
}