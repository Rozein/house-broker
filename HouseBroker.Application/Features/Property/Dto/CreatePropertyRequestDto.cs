using HouseBroker.Domain.Enums;

namespace HouseBroker.Application.Property.Dto;

public class CreatePropertyRequestDto
{
    public decimal Price { get; init; }
    public PropertyType PropertyType { get; init; }
    
    // Optional: existing location id
    public Guid? LocationId { get; init; }
    
    // Location details, required if LocationId is null
    public Guid CityId { get; init; }
    public string Area { get; init; }
    public string PostalCode { get; init; }
    
    public bool IsSold { get; init; }

    public string Feature { get; init; } 
}
