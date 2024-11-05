using AutoMapper;
using GroupService.Interfaces;
using GroupService.Models.Group;
using GroupService.Models.Group.DTOs;
using GroupService.Models.Group.Mapping;
using Moq;
using Shared.Events.Groups;
using Shared.Interfaces;
using Shouldly;

namespace SmartCharging.Tests.UnitTests;

public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly IMapper _mapper;
    private readonly IGroupService _groupService;

    public GroupServiceTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GroupMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _groupService = new GroupService.Services.GroupService(
            _groupRepositoryMock.Object,
            _eventPublisherMock.Object,
            _mapper
        );
    }

    [Fact]
    public async Task DeleteAsync_GroupExists_ShouldPublishEvent()
    {
        // Arrange mock Group
        var groupId = Guid.NewGuid();
        _groupRepositoryMock.Setup(repo => repo.GetByIdAsync(groupId)).ReturnsAsync(new Group { Id = groupId });

        // Act mock Group deletion
        await _groupService.DeleteAsync(groupId);

        // Assert that the appropriate event was published
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.IsAny<GroupDeletedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CapacityLessThanCurrentLoad_ShouldThrowException()
    {
        // Arrange mock Group
        var groupId = Guid.NewGuid();
        var existingGroup = new Group { Id = groupId, CurrentLoad = 50, Capacity = 100 };
        _groupRepositoryMock.Setup(repo => repo.GetByIdAsync(groupId)).ReturnsAsync(existingGroup);

        var updateDto = new UpdateGroupDto { Capacity = 30 };

        // Act mock Group update & Assert that exception is thrown
        await Should.ThrowAsync<ArgumentException>(async () => await _groupService.UpdateAsync(groupId, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ValidUpdate_ShouldUpdateGroup()
    {
        // Arrange mock Group
        var groupId = Guid.NewGuid();
        var existingGroup = new Group { Id = groupId, Name = "Group1", Capacity = 100, CurrentLoad = 20 };
        _groupRepositoryMock.Setup(repo => repo.GetByIdAsync(groupId)).ReturnsAsync(existingGroup);
        var updateDto = new UpdateGroupDto { Name = "UpdatedGroup", Capacity = 150 };

        // Act mock Group update
        var result = await _groupService.UpdateAsync(groupId, updateDto);

        // Assert that mock Group was succesfully updated
        result.Name.ShouldBe("UpdatedGroup");
        result.Capacity.ShouldBe(150);
    }
}