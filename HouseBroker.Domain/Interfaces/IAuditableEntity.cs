namespace HouseBroker.Domain.Interfaces;

public interface IAuditableEntity 
{
    public DateTime CreationTime { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ?LastModifiedTime { get; set; }
    public Guid ?LastModifiedBy { get; set; }
}

