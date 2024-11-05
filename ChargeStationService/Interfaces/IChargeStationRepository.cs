using ChargeStationService.Models.ChargeStation;
using Shared.Interfaces;

namespace ChargeStationService.Interfaces;

public interface IChargeStationRepository : IRepository<ChargeStation>
{
    Task DeleteByGroupIdAsync(Guid groupId);
    Task<IEnumerable<ChargeStation>> GetByGroupIdAsync(Guid groupId);
}