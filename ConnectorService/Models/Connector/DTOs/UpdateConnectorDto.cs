using System.ComponentModel.DataAnnotations;

namespace ConnectorService.Models.Connector.DTOs;

public class UpdateConnectorDto
{
    public Guid? ChargeStationId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Max Current must be greater than zero.")]
    public int? MaxCurrent { get; set; }
}
