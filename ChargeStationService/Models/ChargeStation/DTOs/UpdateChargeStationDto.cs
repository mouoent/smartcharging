using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ChargeStationService.Models.ChargeStation.DTOs;

public class UpdateChargeStationDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    public Guid? GroupId { get; set; }
   
}
