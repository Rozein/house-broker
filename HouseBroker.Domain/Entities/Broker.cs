using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HouseBroker.Domain.Entities.Shared;
using HouseBroker.Domain.Interfaces;

namespace HouseBroker.Domain.Entities;

public class Broker :BaseEntity
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    [ForeignKey(nameof(Agency))]
    public Guid? AgencyId { get; set; }
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;
    [MaxLength(100)]
    public virtual Agency? Agency { get; set; }
    public virtual User User { get; set; }
}