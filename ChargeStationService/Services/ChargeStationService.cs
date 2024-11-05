using AutoMapper;
using ChargeStationService.Interfaces;
using ChargeStationService.Models.ChargeStation;
using ChargeStationService.Models.ChargeStation.DTOs;
using Shared.Events.ChargeStations;
using Shared.Interfaces;
using Shared.Services;

namespace ChargeStationService.Services;

public class ChargeStationService : BaseService<ChargeStation, ChargeStationDto, CreateChargeStationDto, UpdateChargeStationDto>, IChargeStationService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IMapper _mapper;
    private readonly IConnectorServiceClient _connectorServiceClient;
    private readonly IGroupServiceClient _groupServiceClient;

    public ChargeStationService(IChargeStationRepository repository,
        IEventPublisher eventPublisher,
        IMapper mapper,
        IConnectorServiceClient connectorServiceClient,
        IGroupServiceClient groupServiceClient)
        : base(repository, mapper)
    {
        _eventPublisher = eventPublisher;
        _chargeStationRepository = repository;
        _mapper = mapper;
        _connectorServiceClient = connectorServiceClient;
        _groupServiceClient = groupServiceClient;
    }

    public override async Task<ChargeStation> AddAsync(CreateChargeStationDto dto)
    {
        var chargeStation = await base.AddAsync(dto);

        var eventMessage = new ChargeStationCreatedEvent
        {
            ChargeStationId = chargeStation.Id,
            Name = chargeStation.Name,
            InitialConnectorCount = dto.InitialConnectorCount,
            GroupId = chargeStation.GroupId
        };

        await _eventPublisher.PublishAsync(eventMessage);

        return chargeStation;
    }

    public override async Task<ChargeStation> UpdateAsync(Guid id, UpdateChargeStationDto dto)
    {
        // Check for the Charge Station's existence
        var existingChargeStation = await _chargeStationRepository.GetByIdAsync(id);
        if (existingChargeStation == null)
        {
            throw new ArgumentException($"Charge Station with ID {id} not found.");
        }
        var oldGroupId = existingChargeStation.GroupId;

        // Check for the Group's existence
        var existingGroup = await _groupServiceClient.GetGroupAsync(existingChargeStation.GroupId);
        if (existingGroup == null)
        {
            throw new ArgumentException($"Group does not exist.");
        }        

        // Check that the Charge Station's connectors don't exceed the Group's capacity
        var totalCapacity = await GetChargeStationCurrentLoad(existingChargeStation);
        if (existingGroup.Capacity <= totalCapacity)
        {
            throw new ArgumentException($"Charge Station's capacity exceeds selected Group's total capacity.");
        }

        var newChargeStation = await base.UpdateAsync(id, dto);

        var chargeStationUpdatedEvent = new ChargeStationUpdatedEvent
        {
            ChargeStationId = id,
            OldGroupId = oldGroupId,
            NewGroupId = dto.GroupId ?? Guid.Empty,
            ChargeStationTotalCapacity = totalCapacity,
        };

        await _eventPublisher.PublishAsync(chargeStationUpdatedEvent);

        return await base.UpdateAsync(id, dto);
    }

    public override async Task DeleteAsync(Guid chargeStationId)
    {
        await base.DeleteAsync(chargeStationId);

        var chargeStationDeletedEvent = new ChargeStationDeletedEvent { ChargeStationId = chargeStationId };
        await _eventPublisher.PublishAsync(chargeStationDeletedEvent);
    }

    private async Task<int> GetChargeStationCurrentLoad(ChargeStation chargeStation)
    {
        var connectorIds = chargeStation.InternalConnectorIds;
        var totalCapacity = 0;

        if (connectorIds.Count == 0)
            return totalCapacity;

        foreach (var connectorId in connectorIds)
        {
            var connector = await _connectorServiceClient.GetByChargeStationIdAsync(chargeStation.Id, connectorId);
            if (connector == null) continue;
            totalCapacity += connector.MaxCurrent;
        }

        return totalCapacity;
    }
}
