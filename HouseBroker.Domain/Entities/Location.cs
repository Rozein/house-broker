using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HouseBroker.Domain.Entities.Shared;

namespace HouseBroker.Domain.Entities;

public class Location : BaseEntity
{
    [ForeignKey(nameof(City))]
    public Guid CityId { get; set; }
    [MaxLength(100)]
    public string Area { get; set; } = string.Empty;
    [MaxLength(50)]
    public string PostalCode { get; set; } = string.Empty;
    
    public virtual City City { get; set; }
    public List<Property> PropertyListings { get; set; } = new();
}