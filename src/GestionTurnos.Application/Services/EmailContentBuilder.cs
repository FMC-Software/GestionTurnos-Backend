using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Services
{
    public class EmailContentBuilder : IEmailContentBuilder
    {
        public EmailMessage BuildExpiredEmail(string email, string businessName)
        {
            return new EmailMessage
            {
                To = email,
                Subject = $"{businessName} - Suscripción Vencida",
                Body = "Te informamos que tu suscripción ha vencido. Para seguir utilizando la plataforma, por favor renueva tu plan."
            };
        }

        public EmailMessage BuildVencimientoEmail(string email, string businessName, int daysLeft)
        {
            return new EmailMessage
            {
                To = email,
                Subject = $"{businessName} - Próximo Vencimiento de Suscripción",
                Body = $"Te informamos que tu suscripción vencerá en {daysLeft} días. Para evitar interrupciones en el servicio, te recomendamos renovarla antes de la fecha de vencimiento."
            };
        }

        public EmailMessage BuildResetPassword(User user, string token)
        {
            return new EmailMessage
            {
                To = user.Email,
                Subject = "Restablecimiento de Contraseña",
                Body = $@"Hola {user.Name},

                        Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.

                        Para crear una nueva contraseña, haz clic en el siguiente enlace:

                        https://www.FCMTurniFy.com/reset-password?token={token}

                        Por motivos de seguridad, este enlace tiene una validez limitada y solo puede utilizarse una vez.

                        Si no solicitaste el restablecimiento de tu contraseña, puedes ignorar este correo. Tu cuenta permanecerá segura y no se realizará ningún cambio.

                        Atentamente,
                        Equipo de FCM TurniFy"
            };
        }
        public EmailMessage BuildAppointmentConfirmationEmail(string clientEmail, string clientName, string businessName, string branchAddress, string staffName, DateTime day, TimeSpan time)
        {
            string formattedDate = day.ToString("dd/MM/yyyy");
            string formattedTime = time.ToString(@"hh\:mm");

            var details = new[]
            {
                ("Negocio", businessName),
                ("Dirección", branchAddress),
                ("Atendido por", staffName),
                ("Fecha", formattedDate),
                ("Hora", formattedTime),
            };

            return new EmailMessage
            {
                To = clientEmail,
                Subject = $"Turno confirmado en {businessName}",
                Body = $@"Hola {clientName},
                        Te confirmamos que tu turno en {businessName} ha sido reservado exitosamente.
                        Detalles de tu turno:
                        - Negocio: {businessName}
                        - Dirección: {branchAddress}
                        - Atendido por: {staffName}
                        - Fecha: {formattedDate}
                        - Hora: {formattedTime}
                        Si necesitas modificar o cancelar tu turno, por favor contáctanos con anticipación.
                        ¡Gracias por elegirnos!
                        ",
                HtmlBody = BuildAppointmentEmailHtml(
                    headerColor: "#2563eb",
                    headerText: "Turno confirmado",
                    clientName: clientName,
                    introText: $"Te confirmamos que tu turno en <strong>{businessName}</strong> fue reservado exitosamente.",
                    details: details,
                    footerText: "Si necesitás modificar o cancelar tu turno, contactanos con anticipación.",
                    businessName: businessName)
            };
        }

        public EmailMessage BuildAppointmentCancelledEmail(string clientEmail, string clientName, string businessName, string branchAddress, DateTime day, TimeSpan time)
        {
            string formattedDate = day.ToString("dd/MM/yyyy");
            string formattedTime = time.ToString(@"hh\:mm");

            var details = new[]
            {
                ("Negocio", businessName),
                ("Dirección", branchAddress),
                ("Fecha", formattedDate),
                ("Hora", formattedTime),
            };

            return new EmailMessage
            {
                To = clientEmail,
                Subject = $"Turno cancelado en {businessName}",
                Body = $@"Hola {clientName},
                        Lamentablemente tu turno en {businessName} ha sido cancelado.
                        Detalles del turno cancelado:
                        - Negocio: {businessName}
                        - Dirección: {branchAddress}
                        - Fecha: {formattedDate}
                        - Hora: {formattedTime}
                        Podés pedir otro turno con anticipación cuando quieras.
                        ¡Te esperamos!
                        ",
                HtmlBody = BuildAppointmentEmailHtml(
                    headerColor: "#dc2626",
                    headerText: "Turno cancelado",
                    clientName: clientName,
                    introText: $"Lamentablemente tu turno en <strong>{businessName}</strong> fue cancelado.",
                    details: details,
                    footerText: "Podés pedir otro turno con anticipación cuando quieras.",
                    businessName: businessName)
            };
        }

        private static string BuildAppointmentEmailHtml(
            string headerColor,
            string headerText,
            string clientName,
            string introText,
            (string Label, string Value)[] details,
            string footerText,
            string businessName)
        {
            var rows = new StringBuilder();
            for (int i = 0; i < details.Length; i++)
            {
                var borderTop = i == 0 ? string.Empty : "border-top:1px solid #e5e7eb;";
                rows.Append($@"
                    <tr>
                        <td style=""padding:12px 16px;font-size:14px;color:#6b7280;width:140px;{borderTop}"">{details[i].Label}</td>
                        <td style=""padding:12px 16px;font-size:14px;color:#111827;font-weight:600;{borderTop}"">{details[i].Value}</td>
                    </tr>");
            }

            return $@"
<!DOCTYPE html>
<html lang=""es"">
<body style=""margin:0;padding:0;background-color:#f4f5f7;font-family:Segoe UI, Arial, sans-serif;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f5f7;padding:24px 0;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:8px;overflow:hidden;"">
          <tr>
            <td style=""background-color:{headerColor};padding:24px 32px;"">
              <span style=""color:#ffffff;font-size:20px;font-weight:600;"">{headerText}</span>
            </td>
          </tr>
          <tr>
            <td style=""padding:32px;"">
              <p style=""margin:0 0 16px;font-size:16px;color:#1f2937;"">Hola <strong>{clientName}</strong>,</p>
              <p style=""margin:0 0 24px;font-size:15px;color:#374151;line-height:1.5;"">{introText}</p>
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border:1px solid #e5e7eb;border-radius:6px;"">
                {rows}
              </table>
              <p style=""margin:24px 0 0;font-size:13px;color:#6b7280;line-height:1.5;"">{footerText}</p>
            </td>
          </tr>
          <tr>
            <td style=""background-color:#f9fafb;padding:16px 32px;text-align:center;"">
              <span style=""font-size:12px;color:#9ca3af;"">{businessName}</span>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }
    }
}