using ConnectorService.Models.Connector;
using Shared.Interfaces;

namespace ConnectorService.Interfaces
{
    public interface IConnectorRepository : IRepository<Connector>
    {
        Task DeleteByChargeStationIdAsync(Guid chargeStationId);
        Task<IEnumerable<Connector>> GetByChargeStationIdAsync(Guid chargeStationId);
        Task<Connector> GetByChargeStationIdAsync(Guid chargeStationId, int internalConnectorId);
    }
}