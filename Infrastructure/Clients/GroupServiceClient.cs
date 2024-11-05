using Shared.Interfaces;
using Shared.Models.DTOs;
using System.Text.Json;

namespace Infrastructure.Clients;

public class GroupServiceClient : IGroupServiceClient
{
    private readonly HttpClient _httpClient;

    public GroupServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GroupContract> GetGroupAsync(Guid groupId)
    {
        var response = await _httpClient.GetAsync($"/api/groups/{groupId}");
        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var content = await response.Content.ReadAsStringAsync();
        var group = JsonSerializer.Deserialize<GroupContract>(content, options);

        return group;
    }
}
