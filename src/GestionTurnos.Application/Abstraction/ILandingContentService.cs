using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface ILandingContentService
    {
        Task<LandingContentResponse> Get();
        Task<LandingContentResponse> Update(LandingContentRequest request);
    }
}
