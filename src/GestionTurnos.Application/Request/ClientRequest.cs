using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class ClientRequest
    {
        
        public  string? Name { get; set; } = string.Empty;
    
        public  string? Email { get; set; } = string.Empty;


        public  string? Phone { get; set; } = string.Empty;

        public  string? BirthDay { get; set; } = string.Empty;

    }
}