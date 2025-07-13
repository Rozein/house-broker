using System.ComponentModel.DataAnnotations;
using HouseBroker.Domain.Entities.Shared;

namespace HouseBroker.Domain.Entities;

public class City : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; }
}