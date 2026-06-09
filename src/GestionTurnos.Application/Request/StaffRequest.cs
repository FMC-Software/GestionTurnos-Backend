using GestionTurnos.Domain.Entities;
using System.ComponentModel.DataAnnotations;


namespace GestionTurnos.Application.Request
{
    public class StaffRequest
    {
        public  string? Name { get; set; } = string.Empty;

        public  string? Email { get; set; } = string.Empty;

        public  string Password { get; set; } = string.Empty;

        [Phone]
        public  string?  Phone { get; set; } = string.Empty;
        public string? LinkPhoto { get; set; } = string.Empty;

        public required Rol Rol { get; set; }

        public Guid? BranchId { get; set; }

    }
}
