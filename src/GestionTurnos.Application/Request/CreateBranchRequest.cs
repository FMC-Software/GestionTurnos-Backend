using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Request
{
    public class CreateBranchRequest
    {
     
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
    }
}
