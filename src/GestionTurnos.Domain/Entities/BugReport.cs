namespace GestionTurnos.Domain.Entities
{
    public enum BugReportType
    {
        Bug,
        Suggestion,
        Other
    }

    public enum BugReportStatus
    {
        New,
        Resolved
    }

    // Reporte enviado desde el boton "Enviar reporte" de la seccion Ayuda
    // del dashboard. Los datos de quien reporta se sacan del JWT (no del
    // body que manda el cliente) para que no se puedan falsear.
    public class BugReport : BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public BugReportType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? StepsToReproduce { get; set; }
        public string Browser { get; set; } = string.Empty;
        public string PageUrl { get; set; } = string.Empty;

        // Snapshot de quien reporto en el momento del reporte (no FK): si el
        // negocio o el usuario se borran despues, el historial del reporte
        // se mantiene igual.
        public Guid? BusinessId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        public BugReportStatus Status { get; set; } = BugReportStatus.New;
    }
}
