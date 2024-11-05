using ChargeStationService.Interfaces;
using ChargeStationService.Models.ChargeStation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChargeStationService.Controllers;

[ApiController]
[Route("api/chargestations")]
public class ChargeStationController : ControllerBase
{
    private readonly IChargeStationService _chargeStationService;

    public ChargeStationController(IChargeStationService chargeStationService)
    {
        _chargeStationService = chargeStationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllChargeStations()
    {
        try
        {
            var chargeStations = await _chargeStationService.GetAllAsync();
            return Ok(chargeStations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChargeStationById(Guid id)
    {
        try
        {
            var chargeStation = await _chargeStationService.GetByIdAsync(id);
            if (chargeStation == null)
                return NotFound("Charge Station not found");

            return Ok(chargeStation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateChargeStation([FromBody] CreateChargeStationDto chargeStation)
    {
        try
        {
            var createdChargeStation = await _chargeStationService.AddAsync(chargeStation);
            return CreatedAtAction(nameof(GetChargeStationById), new { id = createdChargeStation.Id }, createdChargeStation);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating charge station: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateChargeStation(Guid id, [FromBody] UpdateChargeStationDto updatedChargeStation)
    {
        try
        {
            var chargeStation = await _chargeStationService.GetByIdAsync(id);
            if (chargeStation == null)
                return NotFound("Charge Station not found");
            
            var newChargeStation = await _chargeStationService.UpdateAsync(chargeStation.Id, updatedChargeStation);
            return Ok(newChargeStation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChargeStation(Guid id)
    {
        try
        {
            await _chargeStationService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
