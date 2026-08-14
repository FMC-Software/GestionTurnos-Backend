using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        Task<List<Service>> GetByBusinessId(Guid businessId);

        Task<bool> ExistByName(Guid businessId, string name, Guid? excldudeId = null); //Funcion para validar que no se repita el nombre del servicio en el Create y en Update

        
    }
}
