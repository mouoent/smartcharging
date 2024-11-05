using AutoMapper;
using ChargeStationService.Interfaces;
using ChargeStationService.Models.ChargeStation;
using ChargeStationService.Models.ChargeStation.DTOs;
using ChargeStationService.Models.ChargeStation.Mapping;
using GroupService.EventListeners.ChargeStation;
using GroupService.Interfaces;
using GroupService.Models.Group;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RabbitMQ.Client;
using Shared.Events.ChargeStations;
using Shared.Interfaces;
using Shouldly;

namespace SmartCharging.Tests.UnitTests;

public class ChargeStationServiceTests
{
    private readonly Mock<IChargeStationRepository> _chargeStationRepositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly IMapper _mapper;
    private readonly IChargeStationService _chargeStationService;

    public ChargeStationServiceTests()
    {
        _chargeStationRepositoryMock = new Mock<IChargeStationRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ChargeStationMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _chargeStationService = new ChargeStationService.Services.ChargeStationService(
            _chargeStationRepositoryMock.Object,
            _eventPublisherMock.Object,
            _mapper,
            new Mock<IConnectorServiceClient>().Object,
            new Mock<IGroupServiceClient>().Object);
    }

    [Fact]
    public async Task AddAsync_ValidChargeStation_ShouldPublishEvent()
    {
        // Arrange mock CreateChargeStationDto
        var createDto = new CreateChargeStationDto { Name = "Station1", GroupId = Guid.NewGuid(), InitialConnectorCount = 3 };

        // Act add new ChargeStation
        var result = await _chargeStationService.AddAsync(createDto);

        // Assert that ChargeStation was created and event was published
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.Is<ChargeStationCreatedEvent>(e => e.Name == "Station1")), Times.Once);
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldPublishChargeStationDeletedEvent()
    {
        // Arrange mock ChargeStation
        var chargeStationId = Guid.NewGuid();
        _chargeStationRepositoryMock.Setup(repo => repo.GetByIdAsync(chargeStationId)).ReturnsAsync(new ChargeStation { Id = chargeStationId });

        // Act ChargeStation deletion
        await _chargeStationService.DeleteAsync(chargeStationId);

        // Assert that the appropriate event was published
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.Is<ChargeStationDeletedEvent>(e => e.ChargeStationId == chargeStationId)), Times.Once);
    }

    [Fact]
    public async Task ProcessEventAsync_ValidEvent_ShouldUpdateGroups()
    {
        // Arrange 

        // Mock event message
        var eventMessage = new ChargeStationUpdatedEvent
        {
            ChargeStationId = Guid.NewGuid(),
            OldGroupId = Guid.NewGuid(),
            NewGroupId = Guid.NewGuid(),
            ChargeStationTotalCapacity = 50
        };

        // Mock old group in which updated Charge Station used to belong to
        var oldGroup = new Group
        {
            Id = eventMessage.OldGroupId,
            ChargeStationIds = new List<Guid> { eventMessage.ChargeStationId },
            CurrentLoad = 100
        };

        // Mock new group in which updated Charge Station now belongs to
        var newGroup = new Group
        {
            Id = eventMessage.NewGroupId,
            ChargeStationIds = new List<Guid>(),
            CurrentLoad = 20
        };

        // Mock Group repository
        var groupRepositoryMock = new Mock<IGroupRepository>();
        groupRepositoryMock.Setup(repo => repo.GetByIdAsync(eventMessage.OldGroupId)).ReturnsAsync(oldGroup);
        groupRepositoryMock.Setup(repo => repo.GetByIdAsync(eventMessage.NewGroupId)).ReturnsAsync(newGroup);

        // Mock service provider for mock event listener constructor
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IGroupRepository))).Returns(groupRepositoryMock.Object);

        // Mock service scope for mock service scope factory constructor
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

        // Mock service scope factory mock to be returned by mock service provider
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(serviceScopeMock.Object);

        serviceProviderMock.Setup(sp => sp.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);

        // Mock rabbitmq connection for mock event listener constructor
        var connectionMock = new Mock<IConnection>();
        var modelMock = new Mock<IModel>();
        connectionMock.Setup(c => c.CreateModel()).Returns(modelMock.Object);

        // Mock configuration for event listener constructor
        var configuration = new Mock<IConfiguration>();

        // Mock event listener
        var eventListener = new ChargeStationUpdatedEventListener(configuration.Object, connectionMock.Object, serviceProviderMock.Object);

        // Act event processing
        await eventListener.ProcessEventAsync(eventMessage);

        // Assert event was succesfully processed
        oldGroup.CurrentLoad.ShouldBe(50); // Ensure load was reduced
        newGroup.CurrentLoad.ShouldBe(70); // Ensure load was added
        groupRepositoryMock.Verify(repo => repo.UpdateAsync(oldGroup), Times.Once);
        groupRepositoryMock.Verify(repo => repo.UpdateAsync(newGroup), Times.Once);
    }
}