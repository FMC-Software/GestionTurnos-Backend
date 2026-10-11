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
                BrandName = content.BrandName,
                HeroTitleEs = content.HeroTitleEs,
                HeroTitleEn = content.HeroTitleEn,
                HeroDescriptionEs = content.HeroDescriptionEs,
                HeroDescriptionEn = content.HeroDescriptionEn,
                PlansTitleEs = content.PlansTitleEs,
                PlansTitleEn = content.PlansTitleEn,
                PlansSubtitleEs = content.PlansSubtitleEs,
                PlansSubtitleEn = content.PlansSubtitleEn
            };
        }

        public static void UpdateFromRequest(this LandingContent content, LandingContentRequest request)
        {
            content.BrandName = request.BrandName;
            content.HeroTitleEs = request.HeroTitleEs;
            content.HeroTitleEn = request.HeroTitleEn;
            content.HeroDescriptionEs = request.HeroDescriptionEs;
            content.HeroDescriptionEn = request.HeroDescriptionEn;
            content.PlansTitleEs = request.PlansTitleEs;
            content.PlansTitleEn = request.PlansTitleEn;
            content.PlansSubtitleEs = request.PlansSubtitleEs;
            content.PlansSubtitleEn = request.PlansSubtitleEn;
            content.UpdateDateTime = DateTime.Now;
        }
    }
}
