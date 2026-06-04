using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class ClientRequest
    {
        
        public required string Name { get; set; } = string.Empty;

        
        [EmailAddress]
    
        public required string Email { get; set; } = string.Empty;


        public required string Phone { get; set; } = string.Empty;

        public required string BirthDay { get; set; } = string.Empty;
       
    }
}