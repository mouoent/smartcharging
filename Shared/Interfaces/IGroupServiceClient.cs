using Shared.Models.DTOs;

namespace Shared.Interfaces;

public interface IGroupServiceClient
{
    Task<GroupContract> GetGroupAsync(Guid groupId);
}
