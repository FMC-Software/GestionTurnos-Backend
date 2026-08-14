using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IScheduleService
    {
        public Task<ScheduleResponse> CreateSchedule(ScheduleRequest request);

        public Task UpdateSchedule(ScheduleRequest request, Guid id);
    }
}
