using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class UpdateMyStaffProfileRequest
    {
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? LinkPhoto { get; set; }
    }
}
