using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class LandingContentService : ILandingContentService
    {
        private readonly ILandingContentRepository _landingContentRepository;

        public LandingContentService(ILandingContentRepository landingContentRepository)
        {
            _landingContentRepository = landingContentRepository;
        }

        // Fila unica: si todavia no existe (primera vez que se pide), se crea con
        // los mismos textos que hoy estan hardcodeados en el front (translations.js
        // heroTitle/heroDesc), asi la landing nunca queda sin contenido.
        private async Task<LandingContent> GetOrCreate()
        {
            var existing = (await _landingContentRepository.GetAllGlobal()).FirstOrDefault();
            if (existing != null)
                return existing;

            var content = new LandingContent
            {
                BrandName = "Turnify FMC",
                HeroTitleEs = "Gestión de turnos simple para ti y tus clientes",
                HeroTitleEn = "Scheduling made simple for you and your clients",
                HeroDescriptionEs = "Automatiza tus reservas y ahorra horas cada semana.",
                HeroDescriptionEn = "Automate your appointment booking and save hours every week.",
                PlansTitleEs = "Elegí tu plan",
                PlansTitleEn = "Choose your plan",
                PlansSubtitleEs = "Elegí el plan que se ajuste a tu negocio. Podés cambiarlo cuando quieras desde tu panel.",
                PlansSubtitleEn = "Pick the plan that fits your business. You can change it anytime from your dashboard."
            };

            await _landingContentRepository.Add(content);
            return content;
        }

        public async Task<LandingContentResponse> Get()
        {
            var content = await GetOrCreate();
            return content.ToResponse();
        }

        public async Task<LandingContentResponse> Update(LandingContentRequest request)
        {
            var content = await GetOrCreate();
            content.UpdateFromRequest(request);
            await _landingContentRepository.Update(content);
            return content.ToResponse();
        }
    }
}
