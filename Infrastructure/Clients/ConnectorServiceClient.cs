using Shared.Interfaces;
using Shared.Models.DTOs;
using System.Text.Json;

namespace Infrastructure.Clients;

public class ConnectorServiceClient : IConnectorServiceClient
{
    private readonly HttpClient _httpClient;

    public ConnectorServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ConnectorContract>> GetByChargeStationIdAsync(Guid chargeStationId)
    {
        var response = await _httpClient.GetAsync($"/api/connectors/chargestations/{chargeStationId}");

        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var content = await response.Content.ReadAsStringAsync();
        var connectors = JsonSerializer.Deserialize<IEnumerable<ConnectorContract>>(content, options);

        return connectors;
    }

    public async Task<ConnectorContract> GetByChargeStationIdAsync(Guid chargeStationId, int internalId)
    {
        var response = await _httpClient.GetAsync($"/api/connectors/chargestations/{chargeStationId}/{internalId}");

        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var content = await response.Content.ReadAsStringAsync();
        var connector = JsonSerializer.Deserialize<ConnectorContract>(content, options);

        return connector;
    }
}
