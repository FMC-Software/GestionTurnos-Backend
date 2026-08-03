using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllGlobal();
        Task<T?> GetById(Guid Id);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(Guid Id);
    }
}
