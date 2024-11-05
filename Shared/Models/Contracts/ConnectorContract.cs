namespace Shared.Models.DTOs;

public class ConnectorContract
{    
    public Guid ChargeStationId { get; set; }
 
    public int InternalId { get; set; }

    public int MaxCurrent { get; set; }
}
