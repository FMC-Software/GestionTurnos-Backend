using GestionTurnos.Application.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction
{
    public interface IAppointmentNotificationService
    {
        //se usa el task para que sea asincrono y no bloquee el hilo principal.
        Task SendAppointmentConfirmationAsync(AppointmentRequest request,string businessName,string  branchName);

    }
}
