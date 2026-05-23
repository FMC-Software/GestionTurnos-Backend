using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBranchService
    {
        Branch CreateBranch(Branch Branch);
    }
}
