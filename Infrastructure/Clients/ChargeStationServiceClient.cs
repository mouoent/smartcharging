using Shared.Interfaces;
using Shared.Models;
using System.Text.Json;

namespace Infrastructure.Clients;

public class ChargeStationServiceClient : IChargeStationServiceClient
{
    private readonly HttpClient _httpClient;

    public ChargeStationServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChargeStationContract> GetChargeStation(Guid chargeStationId)
    {
        // API endpoint that retrieves the ChargeStation 
        var response = await _httpClient.GetAsync($"/api/chargestations/{chargeStationId}");

        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var content = await response.Content.ReadAsStringAsync();
        var chargeStation = JsonSerializer.Deserialize<ChargeStationContract>(content, options);

        return chargeStation;
    }
}
