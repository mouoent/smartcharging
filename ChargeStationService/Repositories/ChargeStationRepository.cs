using ChargeStationService.Data;
using ChargeStationService.Interfaces;
using ChargeStationService.Models.ChargeStation;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;

namespace ChargeStationService.Repositories;

public class ChargeStationRepository : Repository<ChargeStation>, IChargeStationRepository
{
    public ChargeStationRepository(ChargeStationServiceContext context) : base(context)
    {
    }

    // Delete ChargeStations by Group id
    public async Task DeleteByGroupIdAsync(Guid groupId)
    {
        var chargeStations = await _dbSet.Where(cs => cs.GroupId == groupId).ToListAsync();
        _dbSet.RemoveRange(chargeStations);

        await _context.SaveChangesAsync();
    }

    // Get ChargeStations by Group id
    public async Task<IEnumerable<ChargeStation>> GetByGroupIdAsync(Guid groupId)
    {
        return await _dbSet.Where(cs => cs.GroupId == groupId).ToListAsync();
    }
}
