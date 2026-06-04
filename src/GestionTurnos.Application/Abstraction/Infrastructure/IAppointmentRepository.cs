using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        List<Appointment> GetByBusinessId(Guid businessId);
        List<Appointment> GetByBranchId(Guid branchId, Guid businessId);
        List<Appointment> GetByStaffId(Guid staffId, Guid businessId);
        Service? GetServiceById(Guid serviceId);
        bool ExistsOverlappingAppointment(
            Guid staffId,
            DateTime day,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeAppointmentId = null
        );
        bool ExistsOverlappingAppointmentForClient(
            Guid clientId,
            DateTime day,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeAppointmentId = null
        );
    }
}
