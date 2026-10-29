namespace GestionTurnos.Application.Response
{
    public class HelpContentResponse
    {
        public Guid Id { get; set; }
        public string WhatsappNumber { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
        public string ScheduleTextEs { get; set; } = string.Empty;
        public string ScheduleTextEn { get; set; } = string.Empty;
        public List<FaqResponse> Faqs { get; set; } = new();
    }
}
