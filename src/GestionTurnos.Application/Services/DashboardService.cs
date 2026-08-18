using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionTurnos.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ITenantProvider _tenantProvider;

        public DashboardService(
            IAppointmentRepository appointmentRepository,
            IBranchRepository branchRepository,
            ITenantProvider tenantProvider)
        {
            _appointmentRepository = appointmentRepository;
            _branchRepository = branchRepository;
            _tenantProvider = tenantProvider;
        }

        public async Task<DashboardSummaryResponse> GetDashboard()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontro la empresa");

            var appointments = await _appointmentRepository.GetByBusinessId(businessId);
            var branches = await _branchRepository.GetByBusinessId(businessId);
            var today = DateTime.Today;
            var startMonth = new DateTime(today.Year, today.Month, 1);
            var endMonth = startMonth.AddMonths(1);

            var monthlyRevenue = new List<MonthlyRevenueDto>(6);
            for (var i = 5; i >= 0; i--)
            {
                var monthStart = startMonth.AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1);
                monthlyRevenue.Add(new MonthlyRevenueDto
                {
                    Month = monthStart.ToString("yyyy-MM"),
                    Revenue = appointments
                        .Where(a =>
                            a.Status != AppointmentStatus.Cancelled &&
                            a.Day >= monthStart &&
                            a.Day < monthEnd)
                        .Sum(a => a.TotalCost)
                });
            }

            var currentMonthTotal = monthlyRevenue[^1].Revenue;
            var nextDay = today.AddDays(1);

            var currentMonthAppointments = appointments
                .Where(a => a.Day >= startMonth && a.Day < endMonth)
                .ToList();

            return new DashboardSummaryResponse
            {
                MonthlyRevenue = monthlyRevenue,
                CurrentMonth = new CurrentMonthDto
                {
                    Revenue = currentMonthTotal,
                    EstimatedEarnings = currentMonthAppointments
                        .Where(a => a.Status != AppointmentStatus.Cancelled && a.Day < nextDay)
                        .Sum(a => a.TotalCost),
                    Pending = currentMonthAppointments.Count(a => a.Status == AppointmentStatus.Pending),
                    Confirmed = currentMonthAppointments.Count(a => a.Status == AppointmentStatus.Confirmed),
                    Cancelled = currentMonthAppointments.Count(a => a.Status == AppointmentStatus.Cancelled)
                },
                Branches = branches.Select(branch =>
                {
                    var branchAppointments = currentMonthAppointments
                        .Where(a => a.Staff?.BranchId == branch.Id)
                        .ToList();

                    return new BranchDashboardDto
                    {
                        BranchId = branch.Id,
                        Name = branch.Name,
                        Pending = branchAppointments.Count(a => a.Status == AppointmentStatus.Pending),
                        Confirmed = branchAppointments.Count(a => a.Status == AppointmentStatus.Confirmed),
                        Cancelled = branchAppointments.Count(a => a.Status == AppointmentStatus.Cancelled),
                        MonthRevenue = branchAppointments
                            .Where(a => a.Status != AppointmentStatus.Cancelled)
                            .Sum(a => a.TotalCost)
                    };
                }).ToList()
            };
        }
    }
}