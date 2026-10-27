using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Response
{
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string? Cc { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
