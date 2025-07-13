using FluentAssertions;
using HouseBroker.Application.Features.Property.Command.UploadPropertyAttachment;
using HouseBroker.Application.Features.Property.Command.UploadPropertyImage;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace HouseBroker.UnitTest.Application.Property;

public class UploadPropertyAttachmentCommandHandlerTests
{
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly Mock<IGenericRepository<Domain.Entities.Property>> _mockPropertyRepository;
    private readonly Mock<IGenericRepository<PropertyAttachments>> _mockPropertyAttachmentRepository;
    private readonly Mock<IAppSettings> _mockAppSettings;
    private readonly UploadPropertyAttachmentCommandHandler _handler;

    public UploadPropertyAttachmentCommandHandlerTests()
    {
        _mockImageStorageService = new Mock<IImageStorageService>();
        _mockPropertyRepository = new Mock<IGenericRepository<Domain.Entities.Property>>();
        _mockPropertyAttachmentRepository = new Mock<IGenericRepository<PropertyAttachments>>();
        _mockAppSettings = new Mock<IAppSettings>();
        
        _handler = new UploadPropertyAttachmentCommandHandler(
            _mockImageStorageService.Object,
            _mockPropertyRepository.Object,
            _mockPropertyAttachmentRepository.Object,
            _mockAppSettings.Object
        );
    }

    [Fact]
    public async Task Handle_PropertyNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new UploadPropertyAttachmentCommand
        {
            PropertyId = propertyId,
            FileName = "test.jpg",
            FileContent = new MemoryStream(new byte[] { 1, 2, 3 })
        };

        // Empty property list (property not found)
        var properties = new List<Domain.Entities.Property>();
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();
        _mockPropertyRepository.Setup(r => r.GetAll()).Returns(mockQueryable.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.ErrorMessage.Should().Contain("does not exist");

        // Verify services were not called
        _mockImageStorageService.Verify(s => s.SaveImageAsync(
            It.IsAny<Guid>(), 
            It.IsAny<string>(), 
            It.IsAny<Stream>()), Times.Never);
        
        _mockPropertyAttachmentRepository.Verify(r => r.InsertAsync(It.IsAny<PropertyAttachments>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImageStorageServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new UploadPropertyAttachmentCommand
        {
            PropertyId = propertyId,
            FileName = "test.jpg",
            FileContent = new MemoryStream(new byte[] { 1, 2, 3 })
        };

        var existingProperty = new Domain.Entities.Property { Id = propertyId };
        var properties = new List<Domain.Entities.Property> { existingProperty };
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();
        _mockPropertyRepository.Setup(r => r.GetAll()).Returns(mockQueryable.Object);

        _mockImageStorageService.Setup(s => s.SaveImageAsync(propertyId, command.FileName, command.FileContent))
            .ThrowsAsync(new InvalidOperationException("Storage service error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Be("Storage service error");

        // Verify attachment was not saved
        _mockPropertyAttachmentRepository.Verify(r => r.InsertAsync(It.IsAny<PropertyAttachments>()), Times.Never);
    }
}