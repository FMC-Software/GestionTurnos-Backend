using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Response;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace GestionTurnos.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail = "SuricataMR@gmail.com";
        private readonly string _fromName = "Gestion Turnos";

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            await SendAsync(message.To, message.Subject, string.Empty, message.Body, message.Cc);
        }

        public async Task SendAsync(string toEmail, string subject, string plainText, string htmlContent, string? ccEmail = null)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);

            if (!string.IsNullOrWhiteSpace(ccEmail))
            {
                msg.AddCc(new EmailAddress(ccEmail));
            }

            await client.SendEmailAsync(msg);
        }
    }
}