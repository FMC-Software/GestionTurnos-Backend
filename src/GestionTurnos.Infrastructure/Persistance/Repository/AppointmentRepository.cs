using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(FMCTurnosDbContext context) : base(context)
        {
        }
        public List<Appointment> GetAll()
        {
            return _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted)
                .ToList();
        }

        public new List<Appointment> GetAllGlobal()
        {
            return _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                    .ThenInclude(s => s.Business)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted)
                .ToList();
        }

        public List<Appointment> GetByBusinessId(Guid businessId)
        {
            return _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted && a.Staff.BusinessId == businessId)
                .ToList();
        }

        public List<Appointment> GetByBranchId(Guid branchId, Guid businessId)
        {
            return _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted && a.Staff.BranchId == branchId && a.Staff.BusinessId == businessId)
                .ToList();
        }

        public List<Appointment> GetByStaffId(Guid staffId, Guid businessId)
        {
            return _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted && a.StaffId == staffId && a.Staff.BusinessId == businessId)
                .ToList();
        }

        public override Appointment? GetById(Guid id)
        {
            return _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .FirstOrDefault(a => a.Id == id && !a.IsDeleted);
        }

        public Service? GetServiceById(Guid serviceId)
        {
            return _context.Set<Service>().FirstOrDefault(s => s.Id == serviceId);
        }

        public bool ExistsOverlappingAppointment(Guid staffId, DateTime day, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null)
        {
            var date = day.Date;
            return _dbSet.Any(a => !a.IsDeleted &&
                                   a.Id != excludeAppointmentId &&
                                   a.StaffId == staffId &&
                                   a.Day.Date == date &&
                                   a.StartTime < endTime &&
                                   a.EndTime > startTime);
        }

        public bool ExistsOverlappingAppointmentForClient(Guid clientId, DateTime day, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null)
        {
            var date = day.Date;
            return _dbSet.Any(a => !a.IsDeleted &&
                                   a.Id != excludeAppointmentId &&
                                   a.ClientId == clientId &&
                                   a.Day.Date == date &&
                                   a.StartTime < endTime &&
                                   a.EndTime > startTime);
        }
    }
}
