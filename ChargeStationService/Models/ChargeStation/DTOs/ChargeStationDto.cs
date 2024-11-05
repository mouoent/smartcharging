using System.ComponentModel.DataAnnotations;

namespace ChargeStationService.Models.ChargeStation.DTOs
{
    public class ChargeStationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5, ErrorMessage = "A Charge Station must have between 1 and 5 connectors.")]
        public List<int> InternalConnectorIds { get; set; } = new List<int>();
    }
}
