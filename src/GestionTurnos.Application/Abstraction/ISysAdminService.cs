using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction
{
    public interface ISysAdminService
    {
        Task<SysAdminUser?> GetByEmail(string email);
    }
}


