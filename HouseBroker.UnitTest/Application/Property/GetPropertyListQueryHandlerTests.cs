using FluentAssertions;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Property.Queries;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace HouseBroker.UnitTest.Application.Property;

public class GetPropertyListQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Domain.Entities.Property>> _mockPropertyRepository;
    private readonly Mock<IAppSettings> _mockAppSettings;
    private readonly GetPropertyListQueryHandler _handler;

    public GetPropertyListQueryHandlerTests()
    {
        _mockPropertyRepository = new Mock<IGenericRepository<Domain.Entities.Property>>();
        _mockAppSettings = new Mock<IAppSettings>();
        _handler = new GetPropertyListQueryHandler(_mockPropertyRepository.Object, _mockAppSettings.Object);
    }

    private List<Domain.Entities.Property> GetSampleProperties()
    {
        var cityId1 = Guid.NewGuid();
        var cityId2 = Guid.NewGuid();

        return new List<Domain.Entities.Property>
        {
            new Domain.Entities.Property
            {
                Id = Guid.NewGuid(),
                Price = 300000,
                PropertyType = PropertyType.House,
                IsSold = false,
                Feature = "Cozy house",
                Location = new Location
                {
                    CityId = cityId1,
                    Area = "Downtown",
                    PostalCode = "12345",
                    City = new City { Id = cityId1, Name = "New York" }
                },
                Broker = new Broker
                {
                    User = new User
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john@example.com",
                        PhoneNumber = "123-456-7890",
                        Address = "123 Main St"
                    }
                },
                PropertyAttachments = new List<PropertyAttachments>
                {
                    new PropertyAttachments { FileName = "house1.jpg", ImageUrl = "images/house1.jpg" }
                }
            },
            new Domain.Entities.Property
            {
                Id = Guid.NewGuid(),
                Price = 500000,
                PropertyType = PropertyType.Apartment,
                IsSold = false,
                Feature = "Modern apartment",
                Location = new Location
                {
                    CityId = cityId2,
                    Area = "Uptown",
                    PostalCode = "54321",
                    City = new City { Id = cityId2, Name = "Boston" }
                },
                Broker = new Broker
                {
                    User = new User
                    {
                        FirstName = "Jane",
                        LastName = "Smith",
                        Email = "jane@example.com",
                        PhoneNumber = "098-765-4321",
                        Address = "456 Oak Ave"
                    }
                },
                PropertyAttachments = new List<PropertyAttachments>
                {
                    new PropertyAttachments { FileName = "apt1.jpg", ImageUrl = "images/apt1.jpg" }
                }
            },
            new Domain.Entities.Property
            {
                Id = Guid.NewGuid(),
                Price = 750000,
                PropertyType = PropertyType.House,
                IsSold = true,
                Feature = "Luxury house",
                Location = new Location
                {
                    CityId = cityId1,
                    Area = "Downtown",
                    PostalCode = "12345",
                    City = new City { Id = cityId1, Name = "New York" }
                },
                Broker = new Broker
                {
                    User = new User
                    {
                        FirstName = "Bob",
                        LastName = "Wilson",
                        Email = "bob@example.com",
                        PhoneNumber = "555-123-4567",
                        Address = "789 Pine St"
                    }
                },
                PropertyAttachments = new List<PropertyAttachments>()
            }
        };
    }

    [Fact]
    public async Task Handle_NoFilters_ShouldReturnAllPropertiesWithPagination()
    {
        // Arrange
        var query = new GetPropertyListQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var properties = GetSampleProperties();
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockQueryable.Object);

        _mockAppSettings.Setup(s => s.ImageBaseUrl)
            .Returns("https://example.com/uploads");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().NotBeNull();
        result.Result.Items.Should().HaveCount(3);
        result.Result.TotalCount.Should().Be(3);

        _mockPropertyRepository.Verify(r => r.GetAllNoTracking(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCityIdFilter_ShouldReturnFilteredProperties()
    {
        // Arrange
        var properties = GetSampleProperties(); // Get properties first
        var targetCityId = properties.First().Location.CityId; // Then get the cityId
        
        var query = new GetPropertyListQuery
        {
            CityId = targetCityId,
            PageNumber = 1,
            PageSize = 10
        };

        var mockQueryable = properties.AsQueryable().BuildMockDbSet();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockQueryable.Object);

        _mockAppSettings.Setup(s => s.ImageBaseUrl)
            .Returns("https://example.com/uploads");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().NotBeNull();
        result.Result.Items.Should().HaveCount(2); 
        result.Result.Items.Should().OnlyContain(p => p.Location.CityName == "New York");
    }

    [Fact]
    public async Task Handle_WithPriceRangeFilter_ShouldReturnPropertiesInRange()
    {
        // Arrange
        var query = new GetPropertyListQuery
        {
            MinPrice = 400000,
            MaxPrice = 600000,
            PageNumber = 1,
            PageSize = 10
        };

        var properties = GetSampleProperties();
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockQueryable.Object);

        _mockAppSettings.Setup(s => s.ImageBaseUrl)
            .Returns("https://example.com/uploads");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Items.Should().HaveCount(1); // Only the $500,000 apartment
        result.Result.Items.First().Price.Should().Be(500000);
        result.Result.Items.First().PropertyType.Should().Be(PropertyType.Apartment);
    }

    [Fact]
    public async Task Handle_WithMultipleFilters_ShouldReturnMatchingProperties()
    {
        // Arrange
        var query = new GetPropertyListQuery
        {
            PropertyType = PropertyType.House,
            Area = "Downtown",
            MaxPrice = 400000,
            PageNumber = 1,
            PageSize = 10
        };

        var properties = GetSampleProperties();
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockQueryable.Object);

        _mockAppSettings.Setup(s => s.ImageBaseUrl)
            .Returns("https://example.com/uploads");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Items.Should().HaveCount(1); // Only the $300,000 house in Downtown
        
        var property = result.Result.Items.First();
        property.PropertyType.Should().Be(PropertyType.House);
        property.Location.Area.Should().Be("Downtown");
        property.Price.Should().Be(300000);
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var query = new GetPropertyListQuery
        {
            PageNumber = 1,
            PageSize = 2 // Smaller page size to test pagination
        };

        var properties = GetSampleProperties();
        var mockQueryable = properties.AsQueryable().BuildMockDbSet();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Returns(mockQueryable.Object);

        _mockAppSettings.Setup(s => s.ImageBaseUrl)
            .Returns("https://example.com/uploads");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Result.Items.Should().HaveCount(2); 
        result.Result.TotalCount.Should().Be(3);
        result.Result.HasNextPage.Should().BeTrue();
        result.Result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetPropertyListQuery();

        _mockPropertyRepository.Setup(r => r.GetAllNoTracking())
            .Throws(new InvalidOperationException("Database connection error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Be("Database connection error");
    }
}