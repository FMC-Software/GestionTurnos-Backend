using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Schedule?> GetByBranchIdAndDay(Guid branchId, DayOfWeek dayOfWeek)
        {
            return await _dbSet.FirstOrDefaultAsync(s =>
                s.BranchId == branchId &&
                s.DayOfWeek == dayOfWeek &&
                !s.IsDeleted);
        }

        public async Task<List<Schedule>> GetByBranchId(Guid branchId)
        {
            var schedules = await _dbSet
                .Where(s => s.BranchId == branchId && !s.IsDeleted)
                .ToListAsync();

            // DayOfWeek se persiste como string (conversión global de enums), así que el orden
            // se hace en memoria para respetar el valor numérico real del enum (Domingo..Sábado).
            return schedules
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToList();
        }
    }
}
