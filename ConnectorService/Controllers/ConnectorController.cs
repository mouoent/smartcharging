using ConnectorService.Interfaces;
using ConnectorService.Models.Connector;
using ConnectorService.Models.Connector.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ConnectorService.Controllers;

[ApiController]
[Route("api/connectors")]
public class ConnectorController : ControllerBase
{
    private readonly IConnectorService _connectorService;

    public ConnectorController(IConnectorService connectorService)
    {
        _connectorService = connectorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConnectors()
    {
        try
        {
            var connectors = await _connectorService.GetAllAsync();
            return Ok(connectors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("chargestations/{chargeStationId}")]
    public async Task<IActionResult> GetConnectorByGroupId(Guid chargeStationId)
    {
        try
        {
            var connectors = await _connectorService.GetByChargeStationId(chargeStationId);
            if (connectors == null)
                return NotFound("No Connectors found");

            return Ok(connectors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetConnectorById(Guid id)
    {
        try
        {
            var connector = await _connectorService.GetByIdAsync(id);
            if (connector == null)
                return NotFound("Connector not found");

            return Ok(connector);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("chargestations/{chargeStationId}/{internalConnectorId}")]
    public async Task<IActionResult> GetConnectorByGroupId(Guid chargeStationId, int internalConnectorId)
    {
        try
        {
            var connector = await _connectorService.GetByChargeStationId(chargeStationId, internalConnectorId);
            if (connector == null)
                return NotFound("Connector not found");

            return Ok(connector);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateConnector([FromBody] CreateConnectorDto connector)
    {
        try
        {
            var createdConnector = await _connectorService.AddAsync(connector);
            return CreatedAtAction(nameof(GetConnectorById), new { id = createdConnector.Id }, createdConnector);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating connector: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConnector(Guid id, [FromBody] UpdateConnectorDto updatedConnector)
    {
        try
        {
            var connector = await _connectorService.GetByIdAsync(id);
            if (connector == null)
                return NotFound("Connector not found");
            
            var newConnector = await _connectorService.UpdateAsync(connector.Id, updatedConnector);
            return Ok(newConnector);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("chargestations/{chargeStationId}/{internalConnectorId}")]
    public async Task<IActionResult> UpdateConnector(Guid chargeStationId, int internalConnectorId, [FromBody] UpdateConnectorDto updatedConnector)
    {
        try
        {
            var connector = await _connectorService.GetByChargeStationId(chargeStationId, internalConnectorId);
            if (connector == null)
                return NotFound("Connector not found");

            var newConnector = await _connectorService.UpdateAsync(connector.Id, updatedConnector);
            return Ok(newConnector);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("chargestations/{chargeStationId}/{internalConnectorId}")]
    public async Task<IActionResult> DeleteConnector(Guid chargeStationId, int internalConnectorId)
    {
        try
        {
            var connector = await _connectorService.GetByChargeStationId(chargeStationId, internalConnectorId);
            if (connector == null)
                return NotFound("Connector not found");

            await _connectorService.DeleteAsync(connector.Id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConnector(Guid id)
    {
        try
        {
            await _connectorService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
