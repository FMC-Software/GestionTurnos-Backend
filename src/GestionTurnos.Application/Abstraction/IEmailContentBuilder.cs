using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace GestionTurnos.Application.Abstraction
{
    public interface IEmailContentBuilder
    {
        EmailMessage BuildVencimientoEmail(string email, string businessName, int daysLeft);
        EmailMessage BuildExpiredEmail(string email, string businessName);

        EmailMessage BuildResetPassword(User user, string token);
    }
}
