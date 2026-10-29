using System.ComponentModel.DataAnnotations;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Request
{
    public class BugReportStatusUpdateRequest
    {
        [Required]
        public BugReportStatus Status { get; set; }
    }
}
