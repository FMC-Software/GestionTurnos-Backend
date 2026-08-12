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
        public async Task<List<Appointment>> GetAll()
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
        }

        public new async Task<List<Appointment>> GetAllGlobal()
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                    .ThenInclude(s => s.Business)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByBusinessId(Guid businessId)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted && a.Staff.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByBranchId(Guid branchId, Guid businessId)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted && a.Staff.BranchId == branchId && a.Staff.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByStaffId(Guid staffId, Guid businessId)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Include(a => a.Service)
                .Where(a => !a.IsDeleted && a.StaffId == staffId && a.Staff.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByStaffIdAndDay(Guid staffId, DateTime day)
        {
            var date = day.Date;
            return await _dbSet
                .Where(a => !a.IsDeleted &&
                            a.Status != AppointmentStatus.Cancelled &&
                            a.StaffId == staffId &&
                            a.Day.Date == date)
                .ToListAsync();
        }

        public override async Task<Appointment?> GetById(Guid id)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Staff)
                    .ThenInclude(s => s.Business)
                .Include(a => a.Staff)
                    .ThenInclude(s => s.Branch)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<Service?> GetServiceById(Guid serviceId)
        {
            return await _context.Set<Service>().FirstOrDefaultAsync(s => s.Id == serviceId);
        }

        public async Task<bool> ExistsOverlappingAppointment(Guid staffId, DateTime day, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null)
        {
            var date = day.Date;
            return await _dbSet.AnyAsync(a => !a.IsDeleted &&
                                   a.Status != AppointmentStatus.Cancelled &&
                                   a.Id != excludeAppointmentId &&
                                   a.StaffId == staffId &&
                                   a.Day.Date == date &&
                                   a.StartTime < endTime &&
                                   a.EndTime > startTime);
        }

        public async Task<bool> ExistsOverlappingAppointmentForClient(Guid clientId, DateTime day, TimeSpan startTime, TimeSpan endTime, Guid? excludeAppointmentId = null)
        {
            var date = day.Date;
            return await _dbSet.AnyAsync(a => !a.IsDeleted &&
                                   a.Status != AppointmentStatus.Cancelled &&
                                   a.Id != excludeAppointmentId &&
                                   a.ClientId == clientId &&
                                   a.Day.Date == date &&
                                   a.StartTime < endTime &&
                                   a.EndTime > startTime);
        }
    }
}
