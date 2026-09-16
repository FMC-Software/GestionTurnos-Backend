using System;
using System.Collections.Generic;

namespace GestionTurnos.Application.Response
{
    public class MonthlyRevenueDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class CurrentMonthDto
    {
        public decimal Revenue { get; set; }
        public decimal EstimatedEarnings { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Cancelled { get; set; }
    }

    public class BranchDashboardDto
    {
        public Guid BranchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Cancelled { get; set; }
        public decimal MonthRevenue { get; set; }
    }

    public class PlanUsageDto
    {
        public int StaffCount { get; set; }
        public int MaxStaffAllowed { get; set; }
        public int BranchCount { get; set; }
        public int MaxBranchesAllowed { get; set; }
        public int ServiceCount { get; set; }
        public int MaxServicesAllowed { get; set; }
    }

    public class DashboardSummaryResponse
    {
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
        public CurrentMonthDto CurrentMonth { get; set; } = new();
        public List<BranchDashboardDto> Branches { get; set; } = new();
        public PlanUsageDto PlanUsage { get; set; } = new();
    }
}