using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;

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

        public async Task SendAppointmentCancelledAsync(Appointment appointment)
        {
            try
            {
                var emailMessage = _emailContentBuilder.BuildAppointmentCancelledEmail(
                    appointment.Client.Email,
                    appointment.Client.Name,
                    appointment.Staff.Business.Name,
                    appointment.Staff.Branch.Address,
                    appointment.Day,
                    appointment.StartTime
                );

                await _emailService.SendEmailAsync(emailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando email de cancelación: {ex.Message}");
            }
        }

        public async Task SendAppointmentReassignedAsync(Appointment appointment, string previousStaffName)
        {
            try
            {
                var emailMessage = _emailContentBuilder.BuildAppointmentReassignedEmail(
                    appointment.Client.Email,
                    appointment.Client.Name,
                    appointment.Staff.Business.Name,
                    appointment.Staff.Branch.Address,
                    appointment.Service.Name,
                    appointment.Day,
                    appointment.StartTime,
                    previousStaffName,
                    appointment.Staff.Name
                );

                // Copia al nuevo profesional.
                emailMessage.Cc = appointment.Staff.Email;

                await _emailService.SendEmailAsync(emailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando email de reasignación: {ex.Message}");
            }
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
