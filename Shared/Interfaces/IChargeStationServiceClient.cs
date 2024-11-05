using Shared.Models;

namespace Shared.Interfaces
{
    public interface IChargeStationServiceClient
    {
        Task<ChargeStationContract> GetChargeStation(Guid chargeStationId);
    }
}