using System.ComponentModel.DataAnnotations;

namespace GroupService.Models.Group.DTOs;

public class UpdateGroupDto
{    
    public string? Name { get; set; }
   
    public int? Capacity { get; set; }
}
