namespace GestionTurnos.Application.Response
{
    public class PlanResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public bool IsActive { get; set; }

        public int MaxStaffAllowed { get; set; }
        public int MaxBranchesAllowed { get; set; }
        public int MaxServicesAllowed { get; set; }
    }
}
