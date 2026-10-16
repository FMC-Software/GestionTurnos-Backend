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

        // Foto de perfil del SysAdmin. Vive aca (en vez de en SysAdminUser)
        // porque Users es TPH y agregar una columna propia para SysAdminUser
        // ahi choca con el nombrado automatico de columnas que usa Staff
        // (ver historial de AddLinkPhotoToSysAdminUser) — como de todos modos
        // ya hay una sola fila singleton administrada por SysAdmin, reusamos
        // esta tabla en vez de migrar Users.
        public string LinkPhoto { get; set; } = string.Empty;

        // Paleta de colores de la landing publica (Home.jsx y sus secciones).
        public string PrimaryColor { get; set; } = "#1a1a2e";
        public string BackgroundColor { get; set; } = "#F8F5F0";
        public string AccentColor { get; set; } = "#e2ddd8";
    }
}
