using ChargeStationService.Models.ChargeStation;
using ChargeStationService.Models.ChargeStation.DTOs;
using Shared.Interfaces;

namespace ChargeStationService.Interfaces
{
    public interface IChargeStationService : IBaseService<ChargeStation, ChargeStationDto, CreateChargeStationDto, UpdateChargeStationDto>
    {
        Task<ChargeStation> AddAsync(CreateChargeStationDto dto);
        Task DeleteAsync(Guid chargeStationId);
        Task<ChargeStation> UpdateAsync(Guid id, UpdateChargeStationDto dto);
    }
}