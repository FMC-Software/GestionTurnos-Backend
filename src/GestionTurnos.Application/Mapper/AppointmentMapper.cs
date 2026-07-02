using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class AppointmentMapper
    {
        public static AppointmentResponse ToResponse(this Appointment appointment)
        {
            return new AppointmentResponse
            {
                Id = appointment.Id,

                ClientName = appointment.Client.Name,
                StaffName = appointment.Staff.Name,
                ServiceName = appointment.Service.Name,

                Day = appointment.Day.ToString("yyyy-MM-dd"),
                StartTime = appointment.StartTime.ToString(@"hh\:mm"),
                EndTime = appointment.EndTime.ToString(@"hh\:mm"),

                Observation = appointment.Observation,

                Payment = appointment.Payment.ToString(),
                Status = appointment.Status.ToString(),

                TotalCost = appointment.TotalCost
            };
        }

        public static GlobalAppointmentResponse ToGlobalResponse(this Appointment appointment)
        {
            return new GlobalAppointmentResponse
            {
                Id = appointment.Id,

                ClientName = appointment.Client.Name,
                StaffName = appointment.Staff.Name,
                ServiceName = appointment.Service.Name,

                Day = appointment.Day.ToString("yyyy-MM-dd"),
                StartTime = appointment.StartTime.ToString(@"hh\:mm"),
                EndTime = appointment.EndTime.ToString(@"hh\:mm"),

                Observation = appointment.Observation,

                Payment = appointment.Payment.ToString(),
                Status = appointment.Status.ToString(),

                TotalCost = appointment.TotalCost,

                BusinessId = appointment.Staff.BusinessId,
                BusinessName = appointment.Staff.Business != null ? appointment.Staff.Business.Name : "Desconocido"
            };
        }
        public static Appointment ToEntity(this AppointmentRequest request, Guid clientId, decimal totalCost, TimeSpan endTime)
        {
            return new Appointment
            {
                StaffId = request.StaffId,
                ClientId = clientId,
                ServiceId = request.ServiceId,

                Day = request.Day,
                StartTime = request.StartTime,

                Observation = request.Observation,

                Payment = request.Payment,

                Status = AppointmentStatus.Pending,

                EndTime = endTime,

                TotalCost = totalCost
            };
        }
    }
}