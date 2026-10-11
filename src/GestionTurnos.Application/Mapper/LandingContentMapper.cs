using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class LandingContentMapper
    {
        public static LandingContentResponse ToResponse(this LandingContent content)
        {
            return new LandingContentResponse
            {
                Id = content.Id,
                HeroTitleEs = content.HeroTitleEs,
                HeroTitleEn = content.HeroTitleEn,
                HeroDescriptionEs = content.HeroDescriptionEs,
                HeroDescriptionEn = content.HeroDescriptionEn
            };
        }

        public static void UpdateFromRequest(this LandingContent content, LandingContentRequest request)
        {
            content.HeroTitleEs = request.HeroTitleEs;
            content.HeroTitleEn = request.HeroTitleEn;
            content.HeroDescriptionEs = request.HeroDescriptionEs;
            content.HeroDescriptionEn = request.HeroDescriptionEn;
            content.UpdateDateTime = DateTime.Now;
        }
    }
}
