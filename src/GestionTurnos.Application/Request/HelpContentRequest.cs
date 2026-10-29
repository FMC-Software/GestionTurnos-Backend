using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class HelpContentRequest
    {
        [Required]
        public string WhatsappNumber { get; set; } = string.Empty;

        [Required]
        public string SupportEmail { get; set; } = string.Empty;

        [Required]
        public string ScheduleTextEs { get; set; } = string.Empty;

        [Required]
        public string ScheduleTextEn { get; set; } = string.Empty;
    }
}
