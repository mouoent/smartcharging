using System.ComponentModel.DataAnnotations;

namespace ChargeStationService.Models.ChargeStation.DTOs
{
    public class CreateChargeStationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }        

        [Required]
        public Guid GroupId { get; set; }

        [Range(1, 5, ErrorMessage="There must be at least 1 connector and no more than 5")]
        public int InitialConnectorCount { get; set; } = 1;
    }
}
