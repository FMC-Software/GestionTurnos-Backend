using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class SysAdminDashboardService : ISysAdminDashboardService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IStaffRepository _staffRepository;

        public SysAdminDashboardService(
            IBusinessRepository businessRepository,
            IBusinessSubscriptionRepository subscriptionRepository,
            IAppointmentRepository appointmentRepository,
            IStaffRepository staffRepository)
        {
            _businessRepository = businessRepository;
            _subscriptionRepository = subscriptionRepository;
            _appointmentRepository = appointmentRepository;
            _staffRepository = staffRepository;
        }

        public async Task<SysAdminDashboardResponse> GetDashboard()
        {
            var businesses = await _businessRepository.GetAllGlobal();
            var subscriptions = await _subscriptionRepository.GetAllWithDetails();
            var appointments = await _appointmentRepository.GetAllGlobal();
            var totalUsers = await _staffRepository.CountAllUsers();

            var now = DateTime.UtcNow;

            var overview = new SysAdminOverviewDto
            {
                TotalBusinesses = businesses.Count,
                ActiveBusinesses = businesses.Count(b => b.IsActive == StatusBusiness.Habilitado),
                TotalUsers = totalUsers,
                TotalAppointments = appointments.Count
            };

            var subscriptionsDto = new SysAdminSubscriptionsDto
            {
                Active = subscriptions.Count(s => s.Status == Status.Active),
                Expired = subscriptions.Count(s => s.Status == Status.Expired)
            };

            var currentMonthRevenue = appointments
                .Where(a => a.Status != AppointmentStatus.Cancelled && a.Day.Year == now.Year && a.Day.Month == now.Month)
                .Sum(a => a.TotalCost);

            var revenueEvolution = new List<SysAdminMonthlyRevenueDto>();
            var appointmentTrend = new List<AppointmentTrendDto>();
            var businessGrowth = new List<MonthlyCountDto>();

            // Se aproxima la fecha de alta de cada negocio con la fecha de inicio de su primera
            // suscripcion (InitialBusinessSubscription la crea en el momento del alta), ya que
            // BaseEntity no tiene un campo de fecha de creación.
            var firstSubscriptionPerBusiness = subscriptions
                .GroupBy(s => s.BusinessId)
                .Select(g => g.Min(s => s.StartDate))
                .ToList();

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);

                var monthAppointments = appointments
                    .Where(a => a.Day.Year == monthDate.Year && a.Day.Month == monthDate.Month)
                    .ToList();

                revenueEvolution.Add(new SysAdminMonthlyRevenueDto
                {
                    Month = monthDate.ToString("yyyy-MM"),
                    Revenue = monthAppointments
                        .Where(a => a.Status != AppointmentStatus.Cancelled)
                        .Sum(a => a.TotalCost)
                });

                appointmentTrend.Add(new AppointmentTrendDto
                {
                    Month = monthDate.ToString("yyyy-MM"),
                    Pending = monthAppointments.Count(a => a.Status == AppointmentStatus.Pending),
                    Confirmed = monthAppointments.Count(a => a.Status == AppointmentStatus.Confirmed),
                    Cancelled = monthAppointments.Count(a => a.Status == AppointmentStatus.Cancelled)
                });

                businessGrowth.Add(new MonthlyCountDto
                {
                    Month = monthDate.ToString("yyyy-MM"),
                    Count = firstSubscriptionPerBusiness.Count(d => d.Year == monthDate.Year && d.Month == monthDate.Month)
                });
            }

            var planDistribution = subscriptions
                .Where(s => s.Status == Status.Active)
                .GroupBy(s => s.Plan.Name)
                .Select(g => new PlanDistributionDto { PlanName = g.Key, Count = g.Count() })
                .ToList();

            var topBusinesses = appointments
                .Where(a => a.Staff?.Business != null)
                .GroupBy(a => new { a.Staff.BusinessId, a.Staff.Business.Name })
                .Select(g => new TopBusinessDto
                {
                    BusinessId = g.Key.BusinessId,
                    BusinessName = g.Key.Name,
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(t => t.AppointmentCount)
                .Take(5)
                .ToList();

            return new SysAdminDashboardResponse
            {
                Overview = overview,
                Subscriptions = subscriptionsDto,
                Revenue = new SysAdminRevenueDto { CurrentMonth = currentMonthRevenue, Evolution = revenueEvolution },
                BusinessGrowth = businessGrowth,
                AppointmentTrend = appointmentTrend,
                PlanDistribution = planDistribution,
                TopBusinesses = topBusinesses
            };
        }
    }
}
