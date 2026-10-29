using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class BugReportRepository : BaseRepository<BugReport>, IBugReportRepository
    {
        public BugReportRepository(FMCTurnosDbContext context) : base(context)
        {
        }
    }
}
