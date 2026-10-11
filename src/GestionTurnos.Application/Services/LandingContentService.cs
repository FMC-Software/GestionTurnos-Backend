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
                PlansSubtitleEn = "Pick the plan that fits your business. You can change it anytime from your dashboard.",
                FooterDescriptionEs = "La forma más simple de gestionar turnos para tu negocio.",
                FooterDescriptionEn = "The simplest way to manage appointments for your business.",
                AboutTitleEs = "Quiénes somos",
                AboutTitleEn = "About us",
                // "{brandName}" se reemplaza en el frontend por el BrandName actual,
                // asi el texto no queda desactualizado si se cambia el nombre de marca.
                AboutDescriptionEs = "{brandName} es una plataforma de gestión de turnos pensada para negocios que atienden por reservas: peluquerías, consultorios, estudios, academias y más. Ayudamos a organizar la agenda, el equipo y los clientes desde un solo lugar, para que dediques menos tiempo a la administración y más tiempo a hacer crecer tu negocio.",
                AboutDescriptionEn = "{brandName} is an appointment management platform built for booking-based businesses: salons, clinics, studios, academies and more. We help organize your schedule, team and clients from one place, so you spend less time on admin and more time growing your business."
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
