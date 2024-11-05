using AutoMapper;
using ChargeStationService.Models.ChargeStation.DTOs;
using Shared.Models;

namespace ChargeStationService.Models.ChargeStation.Mapping;

public class ChargeStationMappingProfile : Profile
{
    public ChargeStationMappingProfile()
    {
        // Mapping from Internal Model to API DTOs
        CreateMap<ChargeStation, ChargeStationDto>();

        CreateMap<CreateChargeStationDto, ChargeStation>()
                .ForMember(dest => dest.InternalConnectorIds, opt =>
                    opt.MapFrom(src => Enumerable.Range(1, src.InitialConnectorCount).ToList()));

        CreateMap<UpdateChargeStationDto, ChargeStation>()
                .ForMember(dest => dest.GroupId, opt =>
                    opt.MapFrom((src, dest) => src.GroupId.HasValue && src.GroupId.Value != Guid.Empty
                        ? src.GroupId.Value
                        : dest.GroupId))
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null)); // Ignore null values

        // Mapping to Inter-Service Contracts
        CreateMap<ChargeStation, ChargeStationContract>()
               .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
               .ForMember(dest => dest.InternalConnectorIds, opt => opt.MapFrom(src => src.InternalConnectorIds));
    }    
}
