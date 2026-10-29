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

        [Required]
        public string FooterDescriptionEs { get; set; } = string.Empty;

        [Required]
        public string FooterDescriptionEn { get; set; } = string.Empty;

        // Datos de contacto y redes sociales del footer: todos opcionales,
        // si quedan vacios el footer simplemente no muestra ese icono/dato.
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string LinkedinUrl { get; set; } = string.Empty;

        [Required]
        public string AboutTitleEs { get; set; } = string.Empty;

        [Required]
        public string AboutTitleEn { get; set; } = string.Empty;

        [Required]
        public string AboutDescriptionEs { get; set; } = string.Empty;

        [Required]
        public string AboutDescriptionEn { get; set; } = string.Empty;

        // Paleta de colores de la landing. No incluye LinkPhoto a proposito:
        // la foto del SysAdmin se actualiza aparte (SysAdminService.UpdateMyProfile),
        // no desde este formulario de contenido.
        [Required]
        public string PrimaryColor { get; set; } = string.Empty;

        [Required]
        public string BackgroundColor { get; set; } = string.Empty;

        [Required]
        public string AccentColor { get; set; } = string.Empty;

        // Imagenes por rubro: opcionales, si vienen null/vacias el frontend
        // usa su fallback local.
        public string? ImageUrlBarberia { get; set; }
        public string? ImageUrlSpa { get; set; }
        public string? ImageUrlPeluqueria { get; set; }
        public string? ImageUrlEstetica { get; set; }
        public string? ImageUrlMasajes { get; set; }
        public string? ImageUrlTattoo { get; set; }

        public string? BusinessTypesOrder { get; set; }
        public string? SectionsOrder { get; set; }
    }
}
