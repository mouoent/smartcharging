using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace ConnectorService.Models.Connector;

public class Connector : BaseEntity
{
    [Required]
    public Guid ChargeStationId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Internal ID must be between 1 and 5.")]
    public int InternalId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Max Current must be greater than zero.")]
    public int MaxCurrent { get; set; }
}
