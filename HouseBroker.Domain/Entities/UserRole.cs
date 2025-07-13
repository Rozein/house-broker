using System.ComponentModel.DataAnnotations.Schema;
using HouseBroker.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HouseBroker.Domain.Entities;
public class UserRole : IdentityUserRole<Guid>, IAuditableEntity, ISoftDeleteEntity
{
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
    [NotMapped]
    public string? RoleName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ?LastModifiedTime { get; set; }
    public Guid ?LastModifiedBy { get; set; }
}

