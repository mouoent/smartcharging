using GroupService.Interfaces;
using GroupService.Models.Group.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GroupService.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups()
    {
        try
        {
            var groups = await _groupService.GetAllAsync();
            return Ok(groups);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupById(Guid id)
    {
        try
        {
            var group = await _groupService.GetByIdAsync(id);
            if (group == null)
                return NotFound("Group not found");

            return Ok(group);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto group)
    {
        try
        {
            var createdGroup = await _groupService.AddAsync(group);
            return CreatedAtAction(nameof(GetGroupById), new { id = createdGroup.Id }, group);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating group: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupDto updatedGroup)
    {
        try
        {
            var group = await _groupService.GetByIdAsync(id);
            if (group == null)
                return NotFound("Group not found");

            var newGroup = await _groupService.UpdateAsync(group.Id, updatedGroup);
            return Ok(newGroup);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(Guid id)
    {
        try
        {
            await _groupService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
