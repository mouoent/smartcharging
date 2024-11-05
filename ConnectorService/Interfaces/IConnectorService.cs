using ConnectorService.Models.Connector;
using ConnectorService.Models.Connector.DTOs;
using Shared.Interfaces;

namespace ConnectorService.Interfaces;

public interface IConnectorService : IBaseService<Connector, ConnectorDto, CreateConnectorDto, UpdateConnectorDto>
{
    Task<Connector> AddAsync(CreateConnectorDto connectorDto);
    Task DeleteAsync(Guid connectorId);
    Task<IEnumerable<ConnectorDto>> GetByChargeStationId(Guid chargeStationId);
    Task<Connector> GetByChargeStationId(Guid chargeStationId, int internalConnectorId);
    Task<Connector> UpdateAsync(Guid id, UpdateConnectorDto dto);
}