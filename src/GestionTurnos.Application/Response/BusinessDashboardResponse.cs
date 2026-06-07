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
}

