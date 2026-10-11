using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class LandingContentRequest
    {
        [Required]
        public string BrandName { get; set; } = string.Empty;

        [Required]
        public string HeroTitleEs { get; set; } = string.Empty;

        [Required]
        public string HeroTitleEn { get; set; } = string.Empty;

        [Required]
        public string HeroDescriptionEs { get; set; } = string.Empty;

        [Required]
        public string HeroDescriptionEn { get; set; } = string.Empty;

        [Required]
        public string PlansTitleEs { get; set; } = string.Empty;

        [Required]
        public string PlansTitleEn { get; set; } = string.Empty;

        [Required]
        public string PlansSubtitleEs { get; set; } = string.Empty;

        [Required]
        public string PlansSubtitleEn { get; set; } = string.Empty;
    }
}
