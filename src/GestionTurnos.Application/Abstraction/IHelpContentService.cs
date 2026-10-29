using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IHelpContentService
    {
        Task<HelpContentResponse> Get();
        Task<HelpContentResponse> Update(HelpContentRequest request);
        Task<FaqResponse> AddFaq(FaqRequest request);
        Task<FaqResponse> UpdateFaq(Guid faqId, FaqRequest request);
        Task DeleteFaq(Guid faqId);
    }
}
