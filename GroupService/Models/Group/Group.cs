using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace GroupService.Models.Group;

public class Group : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
    public int Capacity { get; set; }

    public int CurrentLoad { get; set; } = 0;

    public List<Guid> ChargeStationIds { get; set; } = new List<Guid>();
}
