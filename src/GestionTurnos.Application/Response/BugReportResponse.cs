using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Response
{
    public class BugReportResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public BugReportType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? StepsToReproduce { get; set; }
        public string Browser { get; set; } = string.Empty;
        public string PageUrl { get; set; } = string.Empty;
        public Guid? BusinessId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessUrl { get; set; }
        public TypeBusiness? BusinessCategory { get; set; }
        public string? BusinessLogoUrl { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public BugReportStatus Status { get; set; }
    }
}
