using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Response
{
    public class BusinessDetailResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? UrlLogo { get; set; }

        public TypeBusiness TypeBusiness { get; set; }
        public StatusBusiness Status { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public string? CurrentPlan { get; set; }
        public string? SubscriptionStatus { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

        public int BranchCount { get; set; }
        public int StaffCount { get; set; }
        public int ClientCount { get; set; }
        public int AppointmentCount { get; set; }

        public List<BranchDetailResponse> Branches { get; set; } = new();
    }

    public class BranchDetailResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Phone { get; set; }
    }

    /// <summary>
    /// Internal aggregate used to carry the four business counts back from a single
    /// projected repository query.
    /// </summary>
    public class BusinessStatsResult
    {
        public int BranchCount { get; set; }
        public int StaffCount { get; set; }
        public int ClientCount { get; set; }
        public int AppointmentCount { get; set; }
    }
}
