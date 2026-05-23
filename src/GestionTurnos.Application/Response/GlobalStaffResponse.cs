using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Response
{
    public class GlobalStaffResponse : StaffsResponse
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;

    }
}
