using System.ComponentModel.DataAnnotations;

namespace Shared.Models.DTOs;

public class GroupContract
{    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
    public int Capacity { get; set; }

    public int CurrentLoad { get; set; } = 0;

    public List<Guid> ChargeStationIds { get; set; } = new List<Guid>();
}
