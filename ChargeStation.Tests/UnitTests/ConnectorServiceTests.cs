using AutoMapper;
using ConnectorService.Interfaces;
using ConnectorService.Models.Connector;
using ConnectorService.Models.Connector.DTOs;
using ConnectorService.Models.Connector.Mapping;
using Moq;
using Shared.Events.Connectors;
using Shared.Interfaces;
using Shared.Models;
using Shared.Models.DTOs;
using Shouldly;

namespace SmartCharging.Tests.UnitTests;

public class ConnectorServiceTests
{
    private readonly Mock<IConnectorRepository> _connectorRepositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly IMapper _mapper;
    private readonly IConnectorService _connectorService;
    private readonly Mock<IChargeStationServiceClient> _chargeStationServiceClientMock;
    private readonly Mock<IGroupServiceClient> _groupStationServiceClientMock;

    public ConnectorServiceTests()
    {
        _connectorRepositoryMock = new Mock<IConnectorRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _chargeStationServiceClientMock = new Mock<IChargeStationServiceClient>();
        _groupStationServiceClientMock = new Mock<IGroupServiceClient>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ConnectorMappingProfile>();
        });
        _mapper = config.CreateMapper();

        // Configure mock ChargeStationClient to return a mock ChargeStation
        _chargeStationServiceClientMock
            .Setup(client => client.GetChargeStation(It.IsAny<Guid>()))
            .ReturnsAsync(new ChargeStationContract
            {
                GroupId = Guid.NewGuid(),
                InternalConnectorIds = new List<int> { 1, 2, 3 }
            });

        // Configure mock GroupClient to return a mock Group
        _groupStationServiceClientMock
        .Setup(client => client.GetGroupAsync(It.IsAny<Guid>()))
        .ReturnsAsync(new GroupContract
        {
            Capacity = 100,
            CurrentLoad = 50,
            ChargeStationIds = new List<Guid> { Guid.NewGuid() }
        });

        _connectorService = new ConnectorService.Services.ConnectorService(
            _connectorRepositoryMock.Object,
            _eventPublisherMock.Object,
            _groupStationServiceClientMock.Object,
            _chargeStationServiceClientMock.Object,
            _mapper);
    }

    [Fact]
    public async Task AddAsync_ValidConnector_ShouldPublishEvent()
    {
        // Arrange mock Connector
        var chargeStationId = Guid.NewGuid();
        var createDto = new CreateConnectorDto { ChargeStationId = chargeStationId, MaxCurrent = 30 };      

        // Act mock Connector creation
        var result = await _connectorService.AddAsync(createDto);

        // Assert that the mock Connector was created and the appropriate event was published
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.Is<ConnectorCreatedEvent>(e => e.MaxCurrent == 30)), Times.Once);
        result.ShouldNotBeNull();
    }


    [Fact]
    public async Task DeleteAsync_ValidId_ShouldPublishConnectorDeletedEvent()
    {
        // Arrange mock Connector
        var connectorId = Guid.NewGuid();
        var internalId = 1;
        _connectorRepositoryMock.Setup(repo => repo.GetByIdAsync(connectorId)).ReturnsAsync(new Connector { Id = connectorId, ChargeStationId = Guid.NewGuid(), InternalId = internalId });

        // Act mock Connector deletion
        await _connectorService.DeleteAsync(connectorId);

        // Assert that the appropriate event was published
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.Is<ConnectorDeletedEvent>(e => e.InternalConnectorId == internalId)), Times.Once);
    }
   
}
