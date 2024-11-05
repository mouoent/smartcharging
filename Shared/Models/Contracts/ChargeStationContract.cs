using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class ChargeStationContract
{    
    public Guid GroupId { get; set; }

    public List<int> InternalConnectorIds { get; set; } = new List<int>();
}
