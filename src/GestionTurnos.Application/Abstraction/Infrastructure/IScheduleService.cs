using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IScheduleService
    {
        public ScheduleResponse CreateSchedule(ScheduleRequest request);

        public void UpdateSchedule(ScheduleRequest request, Guid id);
    }
}
