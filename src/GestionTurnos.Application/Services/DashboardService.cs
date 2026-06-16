using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GestionTurnos.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBusinessSubscriptionRepository _businessSubscriptionRepository;
        private readonly ITenantProvider _tenantProvider;

        public DashboardService(
            IAppointmentRepository appointmentRepository,
            IBranchRepository branchRepository,
            IStaffRepository staffRepository,
            IServiceRepository serviceRepository,
            IBusinessSubscriptionRepository businessSubscriptionRepository,
            ITenantProvider tenantProvider)
        {
            _appointmentRepository = appointmentRepository;
            _branchRepository = branchRepository;
            _staffRepository = staffRepository;
            _serviceRepository = serviceRepository;
            _businessSubscriptionRepository = businessSubscriptionRepository;
            _tenantProvider = tenantProvider;
        }

        public DashboardResponse GetDashboard()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontro la empresa");

            var appointments = _appointmentRepository.GetByBusinessId(businessId);
            var branches = _branchRepository.GetByBusinessId(businessId);
            var services = _serviceRepository.GetByBusinessId(businessId);
            var staff = _staffRepository.GetAll();
            var subscription = _businessSubscriptionRepository.GetCurrentSubscription(businessId);
            var today = DateTime.Today;
            var startWeek = today.AddDays(-(int)today.DayOfWeek);
            var endWeek = startWeek.AddDays(7);
            var startMonth = new DateTime(
                today.Year,
                today.Month,
                1);
            var endMonth = startMonth.AddMonths(1);

            return new DashboardResponse
            {
                AppointmentsToday = appointments.Count(a =>
                    a.Day.Date == today),

                AppointmentsThisWeek = appointments.Count(a =>
                    a.Day >= startWeek &&
                    a.Day < endWeek),

                PendingAppointments = appointments.Count(a =>
                    a.Status == AppointmentStatus.Pending),

                ConfirmedAppointments = appointments.Count(a =>
                    a.Status == AppointmentStatus.Confirmed),

                CancelledAppointments = appointments.Count(a =>
                    a.Status == AppointmentStatus.Cancelled),

                TotalClients = appointments
                    .Select(a => a.ClientId)
                    .Distinct()
                    .Count(),

                TotalStaff = staff.Count,

                TotalBranches = branches.Count,

                TotalServices = services.Count,

                RevenueThisMonth = appointments
                    .Where(a =>
                        a.Status == AppointmentStatus.Confirmed &&
                        a.Day >= startMonth &&
                        a.Day < endMonth)
                    .Sum(a => a.TotalCost),

                MostRequestedService = appointments
                    .GroupBy(a => a.Service.Name)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault(),

                CurrentPlan = subscription?.Plan?.Name ?? "Sin plan",

                SubscriptionStatus = subscription?.Status.ToString()
                    ?? "Sin suscripción"
            };
        }
    }
}
