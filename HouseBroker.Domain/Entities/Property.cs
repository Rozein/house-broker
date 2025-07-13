using System.ComponentModel.DataAnnotations.Schema;
using HouseBroker.Domain.Entities.Shared;
using HouseBroker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Domain.Entities;

public class Property : BaseEntity
{
    [Precision(18, 2)] 
    public decimal Price { get; set; }
    public PropertyType PropertyType { get; set; }
    [ForeignKey(nameof(Location))]
    public Guid LocationId { get; set; }     
    [ForeignKey(nameof(Broker))]
    public Guid BrokerId { get; set; }
    public bool IsSold { get; set; }
    
    //Assuming Feature field is rich text
    public string Feature { get; set; }
    
    public ICollection<PropertyAttachments> PropertyAttachments { get; set; } = new List<PropertyAttachments>();
    public virtual Location Location { get; set; }  
    public virtual Broker Broker { get; set; }


}