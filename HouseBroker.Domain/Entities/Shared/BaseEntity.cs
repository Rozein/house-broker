using HouseBroker.Domain.Interfaces;

namespace HouseBroker.Domain.Entities.Shared;

public abstract class BaseEntity : IEntity, IAuditableEntity, ISoftDeleteEntity
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? LastModifiedTime { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public Guid? DeletedBy { get; set; }
}