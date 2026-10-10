using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ITenantProvider _tenantProvider;

        public ScheduleService(IScheduleRepository scheduleRepository, IBranchRepository branchRepository, ITenantProvider tenantProvider)
        {
            _scheduleRepository = scheduleRepository;
            _branchRepository = branchRepository;
            _tenantProvider = tenantProvider;
        }

        public async Task<ScheduleResponse> CreateSchedule(ScheduleRequest request)
        {
            var schedule = request.ToEntitySchedule();
            await _scheduleRepository.Add(schedule);
            return schedule.ToResponseSchedule();
        }

        public async Task<List<ScheduleResponse>> GetByBranch(Guid branchId)
        {
            await EnsureBranchBelongsToCurrentBusiness(branchId);

            var schedules = await _scheduleRepository.GetByBranchId(branchId);
            return schedules.Select(s => s.ToResponseSchedule()).ToList();
        }

        public async Task<List<ScheduleResponse>> UpdateBranchSchedules(Guid branchId, UpdateBranchSchedulesRequest request)
        {
            await EnsureBranchBelongsToCurrentBusiness(branchId);

            if (request.Days.Count == 0)
                throw new ConflictException("Debe enviar al menos un día.");

            if (request.Days.Select(d => d.Day).Distinct().Count() != request.Days.Count)
                throw new ConflictException("Hay días repetidos en la configuración.");

            foreach (var day in request.Days.Where(d => d.IsActive))
            {
                if (day.StartTime < TimeSpan.Zero || day.EndTime > TimeSpan.FromHours(24))
                    throw new ConflictException("El horario debe estar entre 00:00 y 24:00.");

                if (day.StartTime >= day.EndTime)
                    throw new ConflictException("La hora de inicio debe ser anterior a la hora de fin.");

                if (day.SlotDurationMinutes <= 0)
                    throw new ConflictException("La duración del turno debe ser mayor a 0.");

                if (day.EndTime - day.StartTime < TimeSpan.FromMinutes(day.SlotDurationMinutes))
                    throw new ConflictException("La duración del turno no puede superar el horario de atención.");
            }

            var existing = await _scheduleRepository.GetByBranchId(branchId);

            foreach (var day in request.Days)
            {
                var schedule = existing.FirstOrDefault(s => s.DayOfWeek == day.Day);

                if (schedule == null)
                {
                    schedule = new Schedule
                    {
                        BranchId = branchId,
                        DayOfWeek = day.Day,
                        StartTime = day.StartTime,
                        EndTime = day.EndTime,
                        SlotDurationMinutes = day.SlotDurationMinutes,
                        IsActive = day.IsActive
                    };
                    await _scheduleRepository.Add(schedule);
                    existing.Add(schedule);
                    continue;
                }

                schedule.StartTime = day.StartTime;
                schedule.EndTime = day.EndTime;
                schedule.SlotDurationMinutes = day.SlotDurationMinutes;
                schedule.IsActive = day.IsActive;
                await _scheduleRepository.Update(schedule);
            }

            return existing
                .OrderBy(s => s.DayOfWeek)
                .Select(s => s.ToResponseSchedule())
                .ToList();
        }

        private async Task EnsureBranchBelongsToCurrentBusiness(Guid branchId)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var branch = await _branchRepository.GetById(branchId)
                ?? throw new ConflictException("Sucursal no encontrada.");

            if (branch.BusinessId != businessId)
                throw new ConflictException("La sucursal no pertenece a su negocio.");
        }
    }
}
