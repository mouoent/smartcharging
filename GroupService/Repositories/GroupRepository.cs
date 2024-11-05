using GroupService.Data;
using GroupService.Interfaces;
using GroupService.Models.Group;
using Shared.Repositories;

namespace GroupService.Repositories;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    public GroupRepository(GroupServiceContext context) : base(context)
    {
    }
}
