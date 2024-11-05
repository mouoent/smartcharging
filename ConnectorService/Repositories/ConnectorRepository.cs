using ConnectorService.Data;
using ConnectorService.Interfaces;
using ConnectorService.Models.Connector;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;

namespace ConnectorService.Repositories;

public class ConnectorRepository : Repository<Connector>, IConnectorRepository
{
    public ConnectorRepository(ConnectorServiceContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Connector>> GetByChargeStationIdAsync(Guid chargeStationId)
    {
        var connectors = await _dbSet.Where(cs => cs.ChargeStationId == chargeStationId).ToListAsync();

        return connectors;
    }

    public async Task<Connector> GetByChargeStationIdAsync(Guid chargeStationId, int internalConnectorId)
    {
        var connector = await _dbSet.FirstOrDefaultAsync(cs => cs.ChargeStationId == chargeStationId && cs.InternalId == internalConnectorId);

        return connector;
    }

    // Delete Connectors by ChargeStation id
    public async Task DeleteByChargeStationIdAsync(Guid chargeStationId)
    {
        var connectors = await _dbSet.Where(cs => cs.ChargeStationId == chargeStationId).ToListAsync();

        _dbSet.RemoveRange(connectors);
        await _context.SaveChangesAsync();
    }
}
