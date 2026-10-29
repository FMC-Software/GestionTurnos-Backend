using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class HelpContentRepository : BaseRepository<HelpContent>, IHelpContentRepository
    {
        public HelpContentRepository(FMCTurnosDbContext context) : base(context)
        {
        }
    }
}
