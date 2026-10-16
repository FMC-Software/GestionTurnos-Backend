using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface ILandingContentService
    {
        Task<LandingContentResponse> Get();
        Task<LandingContentResponse> Update(LandingContentRequest request);

        // Usado por SysAdminService para el perfil del SysAdmin: la foto vive
        // en esta fila singleton (ver comentario en LandingContent.cs), no en
        // el formulario normal de contenido de la landing.
        Task UpdateLinkPhoto(string linkPhoto);
    }
}
