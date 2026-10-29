using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class FaqRequest
    {
        public int Order { get; set; }

        [Required]
        public string QuestionEs { get; set; } = string.Empty;

        [Required]
        public string QuestionEn { get; set; } = string.Empty;

        [Required]
        public string AnswerEs { get; set; } = string.Empty;

        [Required]
        public string AnswerEn { get; set; } = string.Empty;
    }
}
