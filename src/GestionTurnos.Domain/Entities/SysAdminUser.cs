using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Domain.Entities
{
    public class SysAdminUser : User
    {
        private string _password = string.Empty;

        public string Password
        {
            get => _password; set => _password = BCrypt.Net.BCrypt.HashPassword(value);
        }
    }
}
