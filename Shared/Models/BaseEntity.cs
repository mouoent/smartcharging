using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}
