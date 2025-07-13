using HouseBroker.Domain.Enums;

namespace HouseBroker.Application.Features.Property.Dto;

public class PropertyResponseDto
{
    public Guid Id { get; init; }
    public decimal Price { get; init; }
    public PropertyType PropertyType { get; init; }
    public LocationDto Location { get; init; }     
    public bool IsSold { get; init; }
    public string Feature { get; init; }
    
    public BrokerDetail BrokerDetail {get; init;}
    public List<AttachmentDetails> Attachments { get; init; }
}

public class LocationDto
{
    public string CityName { get; init; }
    public string Area { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
}

public class BrokerDetail
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string AgencyName { get; set; }
}


public class AttachmentDetails
{
    public string ImageUrl { get; init; }
    public string FileName { get; init; }
}