using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class ScheduleRepository : BaseRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(FMCTurnosDbContext context) : base(context)
        {
        }

        public Schedule? GetByBranchIdAndDay(Guid branchId, DayOfWeek dayOfWeek)
        {
            return _dbSet.FirstOrDefault(s => 
                s.BranchId == branchId && 
                s.DayOfWeek == dayOfWeek && 
                !s.IsDeleted);
        }
    }
}
