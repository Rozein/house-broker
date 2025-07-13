using FluentAssertions;
using HouseBroker.Application.Features.Property.Queries;
using HouseBroker.Application.Interface;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace HouseBroker.UnitTest.Application.Property;

public class GetPropertyByIdQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Domain.Entities.Property>> _mockPropertyRepository;
    private readonly Mock<IAppSettings> _mockAppSettings;
    private readonly GetPropertyByIdQueryHandler _handler;

    public GetPropertyByIdQueryHandlerTests()
    {
        _mockPropertyRepository = new Mock<IGenericRepository<Domain.Entities.Property>>();
        _mockAppSettings = new Mock<IAppSettings>();
        _handler = new GetPropertyByIdQueryHandler(_mockPropertyRepository.Object, _mockAppSettings.Object);
    }

    private Domain.Entities.Property GetCompletePropertyEntity(Guid propertyId)
    {
        return new Domain.Entities.Property
        {
            Id = propertyId,
            Price = 500000,
            PropertyType = PropertyType.House,
            IsSold = false,
            Feature = "Beautiful house with garden",
            Location = new Location
            {
                Id = Guid.NewGuid(),
                Area = "Downtown",
                PostalCode = "12345",
                City = new City
                {
                    Id = Guid.NewGuid(),
                    Name = "New York"
                }
            },
            Broker = new Broker
            {
                Id = Guid.NewGuid(),
                User = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "+1234567890",
                    Address = "123 Broker Street"
                }
            },
            PropertyAttachments = new List<PropertyAttachments>
            {
                new PropertyAttachments
                {
                    Id = Guid.NewGuid(),
                    FileName = "image1.jpg",
                    ImageUrl = "property-images/image1.jpg"
                },
                new PropertyAttachments
                {
                    Id = Guid.NewGuid(),
                    FileName = "image2.jpg",
                    ImageUrl = "property-images/image2.jpg"
                }
            }
        };
    }

    [Fact]
    public async Task Handle_ValidPropertyId_ShouldReturnPropertyWithCompleteDetails()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);
        var imageBaseUrl = "https://example.com/images";

        var propertyEntity = GetCompletePropertyEntity(propertyId);
        var properties = new List<Domain.Entities.Property> { propertyEntity };
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockQueryable.Object);

        _mockAppSettings.Setup(s => s.ImageBaseUrl)
            .Returns(imageBaseUrl);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().NotBeNull();

        var propertyResponse = result.Result;
        propertyResponse.Id.Should().Be(propertyId);
        propertyResponse.Price.Should().Be(500000);
        propertyResponse.PropertyType.Should().Be(PropertyType.House);
        propertyResponse.IsSold.Should().BeFalse();
        propertyResponse.Feature.Should().Be("Beautiful house with garden");

        // Verify Location details
        propertyResponse.Location.Should().NotBeNull();
        propertyResponse.Location.CityName.Should().Be("New York");
        propertyResponse.Location.Area.Should().Be("Downtown");
        propertyResponse.Location.PostalCode.Should().Be("12345");

        // Verify Broker details
        propertyResponse.BrokerDetail.Should().NotBeNull();
        propertyResponse.BrokerDetail.FullName.Should().Be("John Doe");
        propertyResponse.BrokerDetail.Email.Should().Be("john.doe@example.com");
        propertyResponse.BrokerDetail.PhoneNumber.Should().Be("+1234567890");
        propertyResponse.BrokerDetail.Address.Should().Be("123 Broker Street");

        // Verify Attachments
        propertyResponse.Attachments.Should().HaveCount(2);
        propertyResponse.Attachments[0].FileName.Should().Be("image1.jpg");
        propertyResponse.Attachments[0].ImageUrl.Should().Be($"{imageBaseUrl}/property-images/image1.jpg");
        propertyResponse.Attachments[1].FileName.Should().Be("image2.jpg");
        propertyResponse.Attachments[1].ImageUrl.Should().Be($"{imageBaseUrl}/property-images/image2.jpg");

        _mockPropertyRepository.Verify(r => r.GetAllNoTracking(), Times.Once);
    }

    [Fact]
    public async Task Handle_PropertyNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        // Empty property list (no matching property)
        var properties = new List<Domain.Entities.Property>();
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockQueryable.Object);

        _mockAppSettings.Setup(s => s.ImageBaseUrl)
            .Returns("https://example.com/images");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Result.Should().BeNull();

        _mockPropertyRepository.Verify(r => r.GetAllNoTracking(), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Throws(new InvalidOperationException("Database connection error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Be("Database connection error");
    }
}