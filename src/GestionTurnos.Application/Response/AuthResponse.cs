using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Response
{
    public class AuthResponse
    {
        public string Token { get; set; }

        public string StaffName { get; set; }

        public Rol Rol { get; set; }
        public Guid BusinessId { get; set; } = Guid.Empty;

    }
}
