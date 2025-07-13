namespace HouseBroker.Domain.Interfaces;
public interface ISoftDeleteEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletionTime { get; set; }
    Guid? DeletedBy { get; set; }
}
