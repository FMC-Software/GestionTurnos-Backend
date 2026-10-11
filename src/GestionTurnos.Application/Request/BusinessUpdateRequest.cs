using GestionTurnos.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class BusinessUpdateRequest
    {
        
     
        public  string? Name { get; set; } = string.Empty;

       
        
        public  string? Phone { get; set; } = string.Empty;
        public string? Url { get; set; }

       
        public  string? LogoUrl { get; set; } = string.Empty;

        public StatusBusiness? IsActive { get; set; }

        public TypeBusiness? Category { get; set; }
    }
}