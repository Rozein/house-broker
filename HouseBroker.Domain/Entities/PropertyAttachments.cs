using System.ComponentModel.DataAnnotations.Schema;
using HouseBroker.Domain.Entities.Shared;
using HouseBroker.Domain.Enums;

namespace HouseBroker.Domain.Entities;

public class PropertyAttachments: BaseEntity
{
    [ForeignKey(nameof(PropertyId))]
    public Guid PropertyId { get; set; }
    
    public string ImageUrl { get; set; }
    public string FileName { get; set; }
    public AttachmentType AttachmentType {get; set;}
    
    public Property Property { get; set; } = null!;

}