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

    public class DashboardSummaryResponse
    {
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
        public CurrentMonthDto CurrentMonth { get; set; } = new();
        public List<BranchDashboardDto> Branches { get; set; } = new();
    }
}