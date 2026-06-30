using GestionTurnos.Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction
{
    public interface IDashboardService
    {
        DashboardResponse GetDashboard();
    }
}
