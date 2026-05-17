using GestionTurnos.Application.Abstraction;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchService _branchService;

        public BranchService(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public bool CreateBranch(Branch Branch)
        {
            return _branchService.CreateBranch(Branch);
        }
    }
}
