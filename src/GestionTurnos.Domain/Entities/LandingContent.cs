namespace GestionTurnos.Domain.Entities
{
    // Fila unica (singleton): el contenido editable de la landing publica
    // (hero de Home.jsx), administrado por SysAdmin.
    public class LandingContent : BaseEntity
    {
        public string BrandName { get; set; } = string.Empty;
        public string HeroTitleEs { get; set; } = string.Empty;
        public string HeroTitleEn { get; set; } = string.Empty;
        public string HeroDescriptionEs { get; set; } = string.Empty;
        public string HeroDescriptionEn { get; set; } = string.Empty;
        public string PlansTitleEs { get; set; } = string.Empty;
        public string PlansTitleEn { get; set; } = string.Empty;
        public string PlansSubtitleEs { get; set; } = string.Empty;
        public string PlansSubtitleEn { get; set; } = string.Empty;
        public string FooterDescriptionEs { get; set; } = string.Empty;
        public string FooterDescriptionEn { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string LinkedinUrl { get; set; } = string.Empty;
        public string AboutTitleEs { get; set; } = string.Empty;
        public string AboutTitleEn { get; set; } = string.Empty;
        public string AboutDescriptionEs { get; set; } = string.Empty;
        public string AboutDescriptionEn { get; set; } = string.Empty;
    }
}
