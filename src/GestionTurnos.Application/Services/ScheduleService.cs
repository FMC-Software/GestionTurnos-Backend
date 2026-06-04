using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }
        public ScheduleResponse CreateSchedule(ScheduleRequest request)
        {
            var schedule = request.ToEntitySchedule();
            _scheduleRepository.Add(schedule);
            return schedule.ToResponseSchedule();
        }

        public void UpdateSchedule(ScheduleRequest request, Guid id)
        {
            var schedule = _scheduleRepository.GetById(id) ?? throw new ConflictException("Schedule not found");


            schedule.EndTime = request.EndTime ?? schedule.EndTime;
            schedule.StartTime = request.StartTime ?? schedule.StartTime;
            schedule.IsDeleted = request.IsDeleted ?? schedule.IsDeleted;
            _scheduleRepository.Update(schedule);
        }

    }
}

