using AutoMapper;
using GroupService.Interfaces;
using GroupService.Models.Group;
using GroupService.Models.Group.DTOs;
using Shared.Events.Groups;
using Shared.Interfaces;
using Shared.Services;

namespace GroupService.Services;

public class GroupService : BaseService<Group, GroupDto, CreateGroupDto, UpdateGroupDto>, IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository groupRepository, IEventPublisher eventPublisher, IMapper mapper) : base(groupRepository, mapper)
    {
        _groupRepository = groupRepository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
    }

    public override async Task<Group> UpdateAsync(Guid id, UpdateGroupDto groupDto)
    {
        var group = await _groupRepository.GetByIdAsync(id);

        if (groupDto.Capacity.HasValue)
        {
            if (groupDto.Capacity.Value <= 0)
            {
                throw new ArgumentException("Capacity cannot be equal or less than 0.");
            }

            if (groupDto.Capacity.Value < group.CurrentLoad)
            {
                throw new ArgumentException("Capacity cannot be less than the current load.");
            }
        }

        return await base.UpdateAsync(id, groupDto);
    }

    // Deleting a group warrants an event to be sent to the Event broker
    public override async Task DeleteAsync(Guid groupId)
    {
        await base.DeleteAsync(groupId);

        var groupDeletedEvent = new GroupDeletedEvent { GroupId = groupId };
        await _eventPublisher.PublishAsync(groupDeletedEvent);
    }

}
