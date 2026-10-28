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
        private readonly string _fromEmail = "turnifyfmc@gmail.com";
        private readonly string _fromName = "Gestion Turnos";

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var htmlContent = !string.IsNullOrEmpty(message.HtmlBody)
                ? message.HtmlBody
                : message.Body.Replace("\n", "<br>");

            await SendAsync(message.To, message.Subject, message.Body, htmlContent);
        }

        public async Task SendAsync(string toEmail, string subject, string plainText, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);

            var response = await client.SendEmailAsync(msg);

        }
    }
}