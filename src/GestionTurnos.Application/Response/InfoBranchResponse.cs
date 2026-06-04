using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Response
{
    public class InfoBranchResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public Guid BusinessId { get; set; }

        // 🔥 Agregamos las listas con la información del "ecosistema" de la sucursal
        public List<StaffsResponse> Staff { get; set; } = new();
        public List<ServiceBusinessResponse> Services { get; set; } = new();
        public List<ScheduleResponse> Schedules { get; set; } = new();
    }
}
