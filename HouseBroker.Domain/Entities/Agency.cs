using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HouseBroker.Domain.Entities.Shared;

namespace HouseBroker.Domain.Entities;

public class Agency : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; }
}