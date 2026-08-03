using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Services
{
    public class SysAdminService : ISysAdminService
    {
        private readonly ISysAdminRepository _sysAdminRepository;

        public SysAdminService(ISysAdminRepository sysAdminRepository)
        {
            _sysAdminRepository = sysAdminRepository;
        }

        public async Task<SysAdminUser?> GetByEmail(string email)
        {
            var sysAdmin = await _sysAdminRepository.GetByEmail(email) ?? null;

            return sysAdmin;
        }
    }
}
