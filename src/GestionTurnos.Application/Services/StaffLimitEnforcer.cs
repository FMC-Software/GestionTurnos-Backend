using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class StaffLimitEnforcer : IStaffLimitEnforcer
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public StaffLimitEnforcer(IStaffRepository staffRepository, IAppointmentRepository appointmentRepository)
        {
            _staffRepository = staffRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task ApplyStaffLimitAsync(Guid businessId, Plan plan)
        {
            if (plan.MaxStaffAllowed < 0)
                return;

            var activeStaff = (await _staffRepository.GetByBusinessIdGlobal(businessId))
                .Where(s => s.IsActive)
                .ToList();

            if (activeStaff.Count <= plan.MaxStaffAllowed)
                return;

            // El Admin siempre se conserva (y cuenta para el limite); el resto se ordena por antiguedad.
            var toKeep = activeStaff
                .OrderByDescending(s => s.Rol == Rol.Admin)
                .ThenBy(s => s.CreatedDateTime)
                .Take(Math.Max(plan.MaxStaffAllowed, activeStaff.Count(s => s.Rol == Rol.Admin)))
                .Select(s => s.Id)
                .ToHashSet();

            var toDeactivate = activeStaff.Where(s => !toKeep.Contains(s.Id));

            var today = DateTime.UtcNow.AddHours(-3).Date;

            foreach (var staff in toDeactivate)
            {
                // Primero los turnos: si falla a mitad de camino, el staff sigue activo y el proceso se reintenta.
                var futureAppointments = await _appointmentRepository.GetFutureActiveByStaffId(staff.Id, today);
                foreach (var appointment in futureAppointments)
                {
                    appointment.Status = AppointmentStatus.PendingReassignment;
                    await _appointmentRepository.Update(appointment);
                }

                staff.IsActive = false;
                await _staffRepository.Update(staff);
            }
        }
    }
}
