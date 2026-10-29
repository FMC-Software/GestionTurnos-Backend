namespace GestionTurnos.Application.Response
{
    public class FaqResponse
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string QuestionEs { get; set; } = string.Empty;
        public string QuestionEn { get; set; } = string.Empty;
        public string AnswerEs { get; set; } = string.Empty;
        public string AnswerEn { get; set; } = string.Empty;
    }
}
