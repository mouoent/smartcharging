using Shared.Models.DTOs;

namespace Shared.Interfaces;

public interface IConnectorServiceClient
{
    Task<IEnumerable<ConnectorContract>> GetByChargeStationIdAsync(Guid chargeStationId);
    Task<ConnectorContract> GetByChargeStationIdAsync(Guid chargeStationId, int internalId);
}
