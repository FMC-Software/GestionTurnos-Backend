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
                PlansSubtitleEn = content.PlansSubtitleEn,
                FooterDescriptionEs = content.FooterDescriptionEs,
                FooterDescriptionEn = content.FooterDescriptionEn,
                ContactEmail = content.ContactEmail,
                ContactPhone = content.ContactPhone,
                InstagramUrl = content.InstagramUrl,
                FacebookUrl = content.FacebookUrl,
                TwitterUrl = content.TwitterUrl,
                LinkedinUrl = content.LinkedinUrl,
                AboutTitleEs = content.AboutTitleEs,
                AboutTitleEn = content.AboutTitleEn,
                AboutDescriptionEs = content.AboutDescriptionEs,
                AboutDescriptionEn = content.AboutDescriptionEn
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
            content.FooterDescriptionEs = request.FooterDescriptionEs;
            content.FooterDescriptionEn = request.FooterDescriptionEn;
            content.ContactEmail = request.ContactEmail;
            content.ContactPhone = request.ContactPhone;
            content.InstagramUrl = request.InstagramUrl;
            content.FacebookUrl = request.FacebookUrl;
            content.TwitterUrl = request.TwitterUrl;
            content.LinkedinUrl = request.LinkedinUrl;
            content.AboutTitleEs = request.AboutTitleEs;
            content.AboutTitleEn = request.AboutTitleEn;
            content.AboutDescriptionEs = request.AboutDescriptionEs;
            content.AboutDescriptionEn = request.AboutDescriptionEn;
            content.UpdateDateTime = DateTime.Now;
        }
    }
}
