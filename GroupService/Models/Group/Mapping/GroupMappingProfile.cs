using AutoMapper;
using GroupService.Models.Group.DTOs;
using Shared.Models.DTOs;

namespace GroupService.Models.Group.Mapping;

public class GroupMappingProfile : Profile
{
    public GroupMappingProfile()
    {
        // Mapping from Internal Model to API DTOs
        CreateMap<Group, GroupDto>();
        CreateMap<CreateGroupDto, Group>();
        CreateMap<UpdateGroupDto, Group>()
             .ForMember(dest => dest.Capacity, opt =>
                opt.MapFrom((src, dest) => src.Capacity.HasValue && src.Capacity.Value > 0 ? src.Capacity.Value : dest.Capacity))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Ignore null values

        // Mapping to Inter-Service Contracts
        CreateMap<Group, GroupContract>();
    }
}
