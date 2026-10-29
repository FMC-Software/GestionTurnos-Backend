using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class FaqRepository : BaseRepository<Faq>, IFaqRepository
    {
        public FaqRepository(FMCTurnosDbContext context) : base(context)
        {
        }
    }
}
