using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GestionTurnos.Application.Services
{
    public class AppointmentNotificationService : IAppointmentNotificationService
    {
        private readonly IEmailContentBuilder _emailContentBuilder;
        private readonly IEmailService _emailService;

        public AppointmentNotificationService(IEmailContentBuilder emailContentBuilder, IEmailService emailService) 
        {
            _emailContentBuilder = emailContentBuilder;
            _emailService = emailService;
        }

        public async Task SendAppointmentConfirmationAsync(AppointmentRequest request, string businessName, string branchName)
        {
            
            try 
            {
                var emailMessage = _emailContentBuilder.BuildAppointmentConfirmationEmail(
                    request.ClientEmail,
                    request.ClientName,
                    businessName,
                    branchName,
                    request.Day,
                    request.StartTime
                );

                await _emailService.SendEmailAsync(emailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando email de confirmación: {ex.Message}");
            }
        }
    }
}
