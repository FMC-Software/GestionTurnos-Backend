using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class LandingContentRequest
    {
        [Required]
        public string HeroTitleEs { get; set; } = string.Empty;

        [Required]
        public string HeroTitleEn { get; set; } = string.Empty;

        [Required]
        public string HeroDescriptionEs { get; set; } = string.Empty;

        [Required]
        public string HeroDescriptionEn { get; set; } = string.Empty;
    }
}
