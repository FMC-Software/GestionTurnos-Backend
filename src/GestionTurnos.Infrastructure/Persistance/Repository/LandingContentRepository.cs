using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class LandingContentRepository : BaseRepository<LandingContent>, ILandingContentRepository
    {
        public LandingContentRepository(FMCTurnosDbContext context) : base(context)
        {
        }
    }
}
