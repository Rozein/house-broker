using HouseBroker.Domain.Enums;

namespace HouseBroker.Application.Property.Dto;

public class UpdatePropertyRequestDto
{
    public Guid Id { get; init; }  // Property ID to update

    public decimal Price { get; init; }

    public PropertyType PropertyType { get; init; }

    public Guid? LocationId { get; init; }  // Optional: if not provided, fallback to CityId, Area, PostalCode

    public Guid CityId { get; init; }       // Used only if LocationId is null
    public string Area { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;

    public bool IsSold { get; init; }

    public string Feature { get; init; }
}