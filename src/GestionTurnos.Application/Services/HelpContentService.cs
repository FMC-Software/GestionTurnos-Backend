using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class HelpContentService : IHelpContentService
    {
        private readonly IHelpContentRepository _helpContentRepository;
        private readonly IFaqRepository _faqRepository;

        public HelpContentService(IHelpContentRepository helpContentRepository, IFaqRepository faqRepository)
        {
            _helpContentRepository = helpContentRepository;
            _faqRepository = faqRepository;
        }

        // Fila unica: si todavia no existe (primera vez que se pide), se crea vacia.
        // A diferencia de LandingContent no hay textos hardcodeados previos que
        // migrar aca, por eso arranca en blanco.
        private async Task<HelpContent> GetOrCreate()
        {
            var existing = (await _helpContentRepository.GetAllGlobal()).FirstOrDefault();
            if (existing != null)
                return existing;

            var content = new HelpContent();
            await _helpContentRepository.Add(content);
            return content;
        }

        // Semilla perezosa (mismo patron que GetOrCreate de arriba): si la tabla
        // de FAQs esta vacia (entorno recien levantado: local, deploy-testing o
        // produccion por primera vez), se cargan las 6 preguntas que antes eran
        // el fallback hardcodeado del frontend (Help.jsx). Una vez insertadas
        // quedan en la base y el SysAdmin las administra desde el panel.
        private async Task<List<Faq>> GetFaqsOrSeed()
        {
            var faqs = await _faqRepository.GetAllGlobal();
            if (faqs.Count > 0)
                return faqs;

            var defaults = new[]
            {
                new Faq
                {
                    Order = 1,
                    QuestionEs = "¿Cómo creo un turno?",
                    QuestionEn = "How do I create an appointment?",
                    AnswerEs = "Andá a Appointments y usá el botón de nuevo turno: elegí sucursal, servicio, profesional, fecha y horario, completá los datos del cliente y confirmá. El turno aparece en la lista con estado Pendiente.",
                    AnswerEn = "Go to Appointments and use the new appointment button: pick the branch, service, professional, date and time, fill in the client's details and confirm. The appointment shows up in the list with Pending status."
                },
                new Faq
                {
                    Order = 2,
                    QuestionEs = "¿Cómo cancelo o reprogramo un turno?",
                    QuestionEn = "How do I cancel or reschedule?",
                    AnswerEs = "Entrá en Appointments, seleccioná el turno y usá las acciones de estado para confirmarlo o cancelarlo. Los cambios se reflejan en la agenda al instante.",
                    AnswerEn = "Open Appointments, select the appointment and use the status actions to confirm or cancel it. Changes are reflected in the agenda right away."
                },
                new Faq
                {
                    Order = 3,
                    QuestionEs = "¿Cómo agrego mi equipo, sucursales y servicios?",
                    QuestionEn = "How do I add my team, branches and services?",
                    AnswerEs = "En Managment Business agregás y editás sucursales, profesionales y servicios. Respetá los límites incluidos en tu plan (staff, sucursales y servicios).",
                    AnswerEn = "In Managment Business you can add and edit branches, professionals and services. Keep in mind the limits included in your plan (staff, branches and services)."
                },
                new Faq
                {
                    Order = 4,
                    QuestionEs = "¿Qué incluye mi plan y cómo lo cambio?",
                    QuestionEn = "What does my plan include and how do I change it?",
                    AnswerEs = "En Settings › Subscription ves tu plan actual, su vigencia y los planes disponibles. Al cambiar de plan te llevamos a MercadoPago para pagarlo; los planes gratuitos se aplican al instante. Desde ahí también renovás tu suscripción.",
                    AnswerEn = "In Settings › Subscription you can see your current plan, its validity and the available plans. When you change plan we take you to MercadoPago to pay it; free plans are applied instantly. You can also renew your subscription from there."
                },
                new Faq
                {
                    Order = 5,
                    QuestionEs = "¿Cómo consulto la agenda del día?",
                    QuestionEn = "How do I check my day's agenda?",
                    AnswerEs = "Appointments te deja filtrar por fecha y estado (Pendiente, Aceptado, Cancelado) y muestra un calendario mensual. El panel de Home resume ingresos y turnos.",
                    AnswerEn = "Appointments lets you filter by date and status (Pending, Accepted, Cancelled) and shows a monthly calendar. The Home panel shows a summary of revenue and appointments."
                },
                new Faq
                {
                    Order = 6,
                    QuestionEs = "¿Cómo funcionan las notificaciones?",
                    QuestionEn = "How do notifications work?",
                    AnswerEs = "La campanita del header muestra los turnos pendientes que todavía no viste. Se actualiza en tiempo real, así que un turno nuevo aparece sin recargar la página.",
                    AnswerEn = "The bell in the header shows pending appointments you haven't seen yet. It updates in real time, so a new appointment appears without reloading the page."
                }
            };

            foreach (var faq in defaults)
            {
                await _faqRepository.Add(faq);
            }

            return defaults.ToList();
        }

        public async Task<HelpContentResponse> Get()
        {
            var content = await GetOrCreate();
            var faqs = await GetFaqsOrSeed();
            return content.ToResponse(faqs);
        }

        public async Task<HelpContentResponse> Update(HelpContentRequest request)
        {
            var content = await GetOrCreate();
            content.UpdateFromRequest(request);
            await _helpContentRepository.Update(content);

            var faqs = await GetFaqsOrSeed();
            return content.ToResponse(faqs);
        }

        public async Task<FaqResponse> AddFaq(FaqRequest request)
        {
            var faq = request.ToEntity();
            await _faqRepository.Add(faq);
            return faq.ToResponse();
        }

        public async Task<FaqResponse> UpdateFaq(Guid faqId, FaqRequest request)
        {
            var faq = await _faqRepository.GetById(faqId)
                ?? throw new NotFoundException("No se encontró la pregunta frecuente.");

            faq.UpdateFromRequest(request);
            await _faqRepository.Update(faq);
            return faq.ToResponse();
        }

        public async Task DeleteFaq(Guid faqId)
        {
            var faq = await _faqRepository.GetById(faqId)
                ?? throw new NotFoundException("No se encontró la pregunta frecuente.");

            await _faqRepository.Delete(faq.Id);
        }
    }
}
