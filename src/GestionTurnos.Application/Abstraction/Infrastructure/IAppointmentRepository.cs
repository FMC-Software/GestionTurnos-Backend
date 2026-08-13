using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        Task<List<Appointment>> GetByBusinessId(Guid businessId);
        Task<List<Appointment>> GetByBranchId(Guid branchId, Guid businessId);
        Task<List<Appointment>> GetByStaffId(Guid staffId, Guid businessId);
        Task<List<Appointment>> GetByStaffIdAndDay(Guid staffId, DateTime day);
        Task<List<Appointment>> GetByBranchIdAndDay(Guid businessId, DateTime day, Guid? branchId = null);
        Task<Service?> GetServiceById(Guid serviceId);
        Task<bool> ExistsOverlappingAppointment(
            Guid staffId,
            DateTime day,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeAppointmentId = null
        );
        Task<bool> ExistsOverlappingAppointmentForClient(
            Guid clientId,
            DateTime day,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeAppointmentId = null
        );
    }
}
