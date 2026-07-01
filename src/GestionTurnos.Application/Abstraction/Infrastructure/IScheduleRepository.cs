using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        Schedule? GetByBranchIdAndDay(Guid branchId, DayOfWeek dayOfWeek);
    }
}
