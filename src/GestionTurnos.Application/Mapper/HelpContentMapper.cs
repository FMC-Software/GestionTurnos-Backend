using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class HelpContentMapper
    {
        public static HelpContentResponse ToResponse(this HelpContent content, List<Faq> faqs)
        {
            return new HelpContentResponse
            {
                Id = content.Id,
                WhatsappNumber = content.WhatsappNumber,
                SupportEmail = content.SupportEmail,
                ScheduleTextEs = content.ScheduleTextEs,
                ScheduleTextEn = content.ScheduleTextEn,
                Faqs = faqs
                    .OrderBy(f => f.Order)
                    .Select(f => f.ToResponse())
                    .ToList()
            };
        }

        public static void UpdateFromRequest(this HelpContent content, HelpContentRequest request)
        {
            content.WhatsappNumber = request.WhatsappNumber;
            content.SupportEmail = request.SupportEmail;
            content.ScheduleTextEs = request.ScheduleTextEs;
            content.ScheduleTextEn = request.ScheduleTextEn;
            content.UpdateDateTime = DateTime.Now;
        }

        public static FaqResponse ToResponse(this Faq faq)
        {
            return new FaqResponse
            {
                Id = faq.Id,
                Order = faq.Order,
                QuestionEs = faq.QuestionEs,
                QuestionEn = faq.QuestionEn,
                AnswerEs = faq.AnswerEs,
                AnswerEn = faq.AnswerEn
            };
        }

        public static Faq ToEntity(this FaqRequest request)
        {
            return new Faq
            {
                Order = request.Order,
                QuestionEs = request.QuestionEs,
                QuestionEn = request.QuestionEn,
                AnswerEs = request.AnswerEs,
                AnswerEn = request.AnswerEn
            };
        }

        public static void UpdateFromRequest(this Faq faq, FaqRequest request)
        {
            faq.Order = request.Order;
            faq.QuestionEs = request.QuestionEs;
            faq.QuestionEn = request.QuestionEn;
            faq.AnswerEs = request.AnswerEs;
            faq.AnswerEn = request.AnswerEn;
            faq.UpdateDateTime = DateTime.Now;
        }
    }
}
