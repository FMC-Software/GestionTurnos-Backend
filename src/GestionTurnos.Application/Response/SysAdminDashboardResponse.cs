namespace GestionTurnos.Application.Response
{
    public class SysAdminDashboardResponse
    {
        public SysAdminOverviewDto Overview { get; set; } = new();
        public SysAdminSubscriptionsDto Subscriptions { get; set; } = new();
        public SysAdminRevenueDto Revenue { get; set; } = new();
        public List<MonthlyCountDto> BusinessGrowth { get; set; } = new();
        public List<AppointmentTrendDto> AppointmentTrend { get; set; } = new();
        public List<PlanDistributionDto> PlanDistribution { get; set; } = new();
        public List<TopBusinessDto> TopBusinesses { get; set; } = new();
    }

    public class SysAdminOverviewDto
    {
        public int TotalBusinesses { get; set; }
        public int ActiveBusinesses { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAppointments { get; set; }
    }

    public class SysAdminSubscriptionsDto
    {
        public int Active { get; set; }
        public int Expired { get; set; }
    }

    public class SysAdminRevenueDto
    {
        public decimal CurrentMonth { get; set; }
        public List<SysAdminMonthlyRevenueDto> Evolution { get; set; } = new();
    }

    public class SysAdminMonthlyRevenueDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class MonthlyCountDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AppointmentTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Cancelled { get; set; }
    }

    public class PlanDistributionDto
    {
        public string PlanName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopBusinessDto
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }
}
