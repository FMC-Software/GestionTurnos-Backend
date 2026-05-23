using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GestionTurnos.Application.Response
{
    public class StaffsResponse
    {
        public Guid IdStaff { get; set; }
        public String StaffName { get; set; } = string.Empty;

        [EmailAddress]
        public String StaffEmail { get; set; }  = string.Empty;

        public string StaffLinkPhoto { get; set; } = string.Empty;

        [Phone]
        public double StaffPhone { get; set; }

        public Rol Rol { get; set; }

        public string BranchId { get; set; }

        public string BranchName { get; set; }


    }
}
