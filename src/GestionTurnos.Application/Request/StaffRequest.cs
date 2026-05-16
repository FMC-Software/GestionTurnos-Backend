using GestionTurnos.Domain.Entities;
using System.ComponentModel.DataAnnotations;


namespace GestionTurnos.Application.Request
{
    public class StaffRequest
    {
        public required string Name { get; set; } = string.Empty;

        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        public required string Password { get; set; } = string.Empty;

        [Phone]
        public required string Phone { get; set; } = string.Empty;
        public string? LinkPhoto { get; set; } = string.Empty;

        public Rol Rol { get; set; }

        [Required]
        public string BusinessCategory { get; set; } = string.Empty;
    }
}
