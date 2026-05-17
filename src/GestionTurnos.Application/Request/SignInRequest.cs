using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Request
{
    public class SignInRequest
    {
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; }
    }
}
