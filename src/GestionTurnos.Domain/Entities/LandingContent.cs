namespace GestionTurnos.Domain.Entities
{
    // Fila unica (singleton): el contenido editable de la landing publica
    // (hero de Home.jsx), administrado por SysAdmin.
    public class LandingContent : BaseEntity
    {
        public string HeroTitleEs { get; set; } = string.Empty;
        public string HeroTitleEn { get; set; } = string.Empty;
        public string HeroDescriptionEs { get; set; } = string.Empty;
        public string HeroDescriptionEn { get; set; } = string.Empty;
    }
}
