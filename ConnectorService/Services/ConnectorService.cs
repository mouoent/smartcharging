using AutoMapper;
using ConnectorService.Interfaces;
using ConnectorService.Models.Connector;
using ConnectorService.Models.Connector.DTOs;
using Shared.Events.Connectors;
using Shared.Interfaces;
using Shared.Services;

namespace ConnectorService.Services;

public class ConnectorService : BaseService<Connector, ConnectorDto, CreateConnectorDto, UpdateConnectorDto>, IConnectorService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IConnectorRepository _connectorRepository;
    private readonly IGroupServiceClient _groupServiceClient;
    private readonly IChargeStationServiceClient _chargeStationClientService;
    private readonly IMapper _mapper;

    public ConnectorService(IConnectorRepository connectorRepository,
        IEventPublisher eventPublisher,
        IGroupServiceClient groupServiceClient,
        IChargeStationServiceClient chargeStationClientService,
        IMapper mapper)
        : base(connectorRepository, mapper)
    {
        _connectorRepository = connectorRepository;
        _eventPublisher = eventPublisher;
        _groupServiceClient = groupServiceClient;
        _chargeStationClientService = chargeStationClientService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ConnectorDto>> GetByChargeStationId(Guid chargeStationId)
    {
        var connectors = await _connectorRepository.GetByChargeStationIdAsync(chargeStationId);

        var connectorDtos = _mapper.Map<IEnumerable<ConnectorDto>>(connectors);
        return connectorDtos;
    }

    public async Task<Connector> GetByChargeStationId(Guid chargeStationId, int internalConnectorId)
    {
        var connector = await _connectorRepository.GetByChargeStationIdAsync(chargeStationId, internalConnectorId);
        
        return connector;
    }

    public override async Task<Connector> AddAsync(CreateConnectorDto connectorDto)
    {
        // Retrieve the ChargeStation to access GroupId and connectors
        var chargeStation = await _chargeStationClientService.GetChargeStation(connectorDto.ChargeStationId);
        if (chargeStation == null)
            throw new Exception("Charge Station not found");

        // Find the next available InternalId
        int nextAvailableInternalId = Enumerable.Range(1, 5)
            .Except(chargeStation.InternalConnectorIds)
            .FirstOrDefault();

        if (nextAvailableInternalId == 0)
            throw new Exception("No available Internal ID found for the new connector.");

        // Map the dto to a data model
        var connector = _mapper.Map<Connector>(connectorDto);
        connector.InternalId = nextAvailableInternalId;

        // Check if adding the new connector would exceed the ChargeStation's allowed capacity
        if (chargeStation.InternalConnectorIds.Count() >= 5)
            throw new Exception("Adding this connector would exceed the allowed number of connectors for the Charge Station.");

        // Use GroupId from ChargeStation to retrieve Group from GroupService
        var group = await _groupServiceClient.GetGroupAsync(chargeStation.GroupId);
        if (group == null)
            throw new Exception("Group not found");

        // Validate that adding the new connector will not exceed the group's capacity
        var totalCurrentWithNewConnector = group.CurrentLoad + connectorDto.MaxCurrent;
        if (totalCurrentWithNewConnector > group.Capacity)
            throw new Exception("Adding this connector would exceed the Group's capacity.");

        // Add the connector to the database
        await _connectorRepository.AddAsync(connector);

        // Publish ConnectorAddedEvent        
        var eventMessage = new ConnectorCreatedEvent { GroupId = chargeStation.GroupId, InternalId = connector.InternalId, MaxCurrent = connector.MaxCurrent };
        await _eventPublisher.PublishAsync(eventMessage);

        return connector;
    }

    public override async Task<Connector> UpdateAsync(Guid id, UpdateConnectorDto dto)
    {
        // Retrieve the existing connector
        var existingConnector = await _connectorRepository.GetByIdAsync(id);
        if (existingConnector == null)
            throw new Exception($"Connector with ID {id} not found.");

        int oldInternalId = existingConnector.InternalId;
        Guid oldChargeStationId = existingConnector.ChargeStationId;
        int oldCurrent = existingConnector.MaxCurrent;

        // Check if ChargeStationId is being updated
        if (dto.ChargeStationId.HasValue && dto.ChargeStationId != existingConnector.ChargeStationId)
        {
            // Validate the new ChargeStation
            var newChargeStation = await _chargeStationClientService.GetChargeStation(dto.ChargeStationId.Value);
            if (newChargeStation == null)
                throw new Exception("New Charge Station not found.");

            // Ensure the new ChargeStation has capacity for the connector
            if (newChargeStation.InternalConnectorIds.Count() >= 5)
                throw new Exception("The new Charge Station has reached its connector capacity.");

            // Validate the old charge station
            var oldChargeStation = await _chargeStationClientService.GetChargeStation(oldChargeStationId);

            // Ensure the old ChargeStation won't be empty
            if (oldChargeStation.InternalConnectorIds.Count() <= 1)
                throw new Exception("The new Charge Station can't be left without connectors.");

            // Find the next available InternalId for the new ChargeStation
            int nextAvailableInternalId = Enumerable.Range(1, 5)
                .Except(newChargeStation.InternalConnectorIds)
                .FirstOrDefault();

            if (nextAvailableInternalId == 0)
                throw new Exception("No available Internal ID found for the new ChargeStation.");

            // Update the connector's InternalId            
            existingConnector.InternalId = nextAvailableInternalId;

            // Retrieve the Group associated with the new ChargeStation
            var group = await _groupServiceClient.GetGroupAsync(newChargeStation.GroupId);
            if (group == null)
                throw new Exception("Group not found for the new ChargeStation.");

            // Validate that moving the connector to the new ChargeStation does not exceed the Group's capacity
            var totalCurrentWithNewConnector = group.CurrentLoad + existingConnector.MaxCurrent;
            if (totalCurrentWithNewConnector > group.Capacity)
                throw new Exception("Moving this connector would exceed the Group's capacity.");
        }

        // Map and update other properties if provided in the DTO
        //_mapper.Map(dto, existingConnector);

        dto.ChargeStationId = dto.ChargeStationId ?? existingConnector.ChargeStationId;
        dto.MaxCurrent = dto.MaxCurrent ?? existingConnector.MaxCurrent;                    

        await base.UpdateAsync(existingConnector.Id, dto);

        // Create an event message for notifying the charge station
        var eventMessage = new ConnectorUpdatedEvent
        {
            NewChargeStationId = dto.ChargeStationId ?? Guid.Empty,
            OldChargeStationId = oldChargeStationId,
            OldInternalId = oldInternalId,
            NewInternalId = existingConnector.InternalId,
            NewMaxCurrent = dto.MaxCurrent ?? 0,
            OldMaxCurrent = oldCurrent,
        };

        // Publish the event
        await _eventPublisher.PublishAsync(eventMessage);        

        return existingConnector;
    }

    public override async Task DeleteAsync(Guid connectorId)
    {
        // Retrieve the existing connector
        var connector = await _connectorRepository.GetByIdAsync(connectorId);
        if (connector == null)
            throw new ArgumentException($"Connector with ID {connectorId} not found.");

        var chargeStationId = connector.ChargeStationId;

        // Delete the connector
        await base.DeleteAsync(connectorId);

        // Create an event message for notifying the charge station
        var eventMessage = new ConnectorDeletedEvent
        {
            ChargeStationId = chargeStationId,
            InternalConnectorId = connector.InternalId,
            MaxCurrent = connector.MaxCurrent
        };

        // Publish the event
        await _eventPublisher.PublishAsync(eventMessage);
    }
}

