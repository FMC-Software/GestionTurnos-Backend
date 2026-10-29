using System.ComponentModel.DataAnnotations;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Request
{
    public class BugReportCreateRequest
    {
        [Required]
        public BugReportType Type { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? StepsToReproduce { get; set; }

        [Required]
        public string Browser { get; set; } = string.Empty;

        [Required]
        public string PageUrl { get; set; } = string.Empty;
    }
}
