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
        public EmailMessage BuildAppointmentConfirmationEmail(string clientEmail, string clientName, string businessName, string branchAddress, DateTime day, TimeSpan time)
        {
            string formattedDate = day.ToString("dd/MM/yyyy");
            string formattedTime = time.ToString(@"hh\:mm");
            return new EmailMessage
            {
                To = clientEmail,
                Subject = $"{businessName} - Confirmación de turno en {businessName}",
                Body = $@"Hola {clientName},
                        Te confirmamos que tu turno en {businessName} ha sido reservado exitosamente.
                        Detalles de tu turno:
                        - Negocio: {businessName}
                        - Dirección: {branchAddress}
                        - Fecha: {formattedDate}
                        - Hora: {formattedTime}
                        Si necesitas modificar o cancelar tu turno, por favor contáctanos con anticipación.
                        ¡Gracias por elegirnos!
                        "
            };
        }
    }
}