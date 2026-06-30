using GestionTurnos.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Response
{
    public class BusinessDashboardResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;

        public TypeBusiness Category { get; set; }

    }

    public class BranchResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;
        public string? City { get; set; }
    }

    public class ServiceResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class DashboardResponse
    {
        public int AppointmentsToday { get; set; }
        public int AppointmentsThisWeek { get; set; }
        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int TotalClients { get; set; }
        public int TotalStaff {  get; set; }
        public int TotalBranches { get; set; }
        public int TotalServices { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public string? MostRequestedService { get; set; }
        public string CurrentPlan { get; set; } = string.Empty;
        public string SubscriptionStatus {  get; set; } = string.Empty;
    }
}

