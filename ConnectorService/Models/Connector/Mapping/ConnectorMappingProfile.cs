using AutoMapper;
using ConnectorService.Models.Connector.DTOs;
using Shared.Models.DTOs;

namespace ConnectorService.Models.Connector.Mapping;

public class ConnectorMappingProfile : Profile
{
    public ConnectorMappingProfile()
    {
        // Mapping from Internal Model to API DTOs
        CreateMap<Connector, ConnectorDto>().ReverseMap();

        // Mapping from Create DTO to Internal Model
        CreateMap<CreateConnectorDto, Connector>();

        // Mapping from Update DTO to Internal Model
        CreateMap<UpdateConnectorDto, Connector>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Ignore null values

        // Mapping to Inter-Service Contract
        CreateMap<Connector, ConnectorContract>();
    }
}
