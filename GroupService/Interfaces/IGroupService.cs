using GroupService.Models.Group;
using GroupService.Models.Group.DTOs;
using Shared.Interfaces;

namespace GroupService.Interfaces;

public interface IGroupService : IBaseService<Group, GroupDto, CreateGroupDto, UpdateGroupDto>
{
    Task DeleteAsync(Guid groupId);
    Task<Group> UpdateAsync(Guid id, UpdateGroupDto groupDto);
}