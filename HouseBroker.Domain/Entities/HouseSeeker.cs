using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HouseBroker.Domain.Entities.Shared;

namespace HouseBroker.Domain.Entities;

public class HouseSeeker : BaseEntity
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public virtual User User { get; set; }
    
}