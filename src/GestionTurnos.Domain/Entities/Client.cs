using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestionTurnos.Domain.Entities
{
    public class Client : User
    {
        [Required]
        public Guid BusinessId { get; set; }
        public  Business Business { get; set; } = null!;

        public DateTime BirthDay { get; set; }

        // Propiedades de navegación inversa
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
       


    }
}
