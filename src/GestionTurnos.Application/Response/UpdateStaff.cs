using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Response
{
    public class UpdateStaff
    {
        public string NameStaff { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string LinkPhoto { get; set; } = string.Empty;

        public Rol Rol { get; set; } 

    }
}
