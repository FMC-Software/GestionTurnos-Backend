namespace GestionTurnos.Domain.Entities
{
    // Fila unica (singleton): datos de contacto de la seccion "Ayuda" del
    // dashboard de Admin (WhatsApp, mail de soporte, horario), administrados
    // por SysAdmin. Mismo patron que LandingContent pero para el dashboard
    // en vez de la landing publica.
    public class HelpContent : BaseEntity
    {
        public string WhatsappNumber { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
        public string ScheduleTextEs { get; set; } = string.Empty;
        public string ScheduleTextEn { get; set; } = string.Empty;
    }
}
