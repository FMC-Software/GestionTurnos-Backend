using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IStaffRepository _staffRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IClientRepository clientRepository, IStaffRepository staffRepository)
        {
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
        }

        public List<AppointmentResponse> GetAll()
        {
            var appointments = _appointmentRepository.GetAll();

            return appointments
                .Select(a => a.ToResponse())
                .ToList();
        }

        public AppointmentResponse GetById(Guid id)
        {
            var appointment = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Turno no encontrado.");
            return appointment.ToResponse();
        }

        public AppointmentResponse CreateAppointment(AppointmentRequest request)
        {
            // 1. Obtener el Staff para derivar el BusinessId
            var staff = _staffRepository.GetById(request.StaffId)
                ?? throw new Exception("El profesional no fue encontrado.");

            // 2. Buscar cliente por email. Si no existe, crearlo.
            var client = _clientRepository.GetClientByEmail(request.ClientEmail);

            if (client == null)
            {
                client = new Client
                {
                    Name = request.ClientName,
                    Email = request.ClientEmail,
                    Phone = request.ClientPhone,
                    BirthDay = request.ClientBirthDay,
                    BusinessId = staff.BusinessId,
                    UpdateDateTime = DateTime.UtcNow
                };
                _clientRepository.Add(client);
            }

            var clientId = client.Id;

            // 2. Verificar solapamiento de horarios
            var endTime = request.StartTime.TimeOfDay.Add(TimeSpan.FromHours(1));

            if (_appointmentRepository.ExistsOverlappingAppointment(request.StaffId, request.Day, request.StartTime.TimeOfDay, endTime))
            {
                throw new Exception("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (_appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime.TimeOfDay, endTime))
            {
                throw new Exception("El cliente ya tiene un turno asignado en ese horario.");
            }

            // 3. Crear el turno con el clientId resuelto
            var appointment = request.ToEntity(clientId);
            var appointmentCreated = _appointmentRepository.Add(appointment);

            var fullyLoaded = _appointmentRepository.GetById(appointmentCreated.Id) 
                ?? throw new Exception("Error al cargar el turno creado.");

            return fullyLoaded.ToResponse();
        }

        public AppointmentResponse UpdateAppointment(Guid id, AppointmentRequest request)
        {
            var existing = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Turno no encontrado.");

            // Obtener el Staff para derivar el BusinessId
            var staff = _staffRepository.GetById(request.StaffId)
                ?? throw new Exception("El profesional no fue encontrado.");

            // Resolver el cliente por email (find or create)
            var client = _clientRepository.GetClientByEmail(request.ClientEmail);
            if (client == null)
            {
                client = new Client
                {
                    Name = request.ClientName,
                    Email = request.ClientEmail,
                    Phone = request.ClientPhone,
                    BirthDay = request.ClientBirthDay,
                    BusinessId = staff.BusinessId,
                    UpdateDateTime = DateTime.UtcNow
                };
                _clientRepository.Add(client);
            }

            var clientId = client.Id;
            var endTime = request.StartTime.TimeOfDay.Add(TimeSpan.FromHours(1));

            if (_appointmentRepository.ExistsOverlappingAppointment(request.StaffId, request.Day, request.StartTime.TimeOfDay, endTime, id))
            {
                throw new Exception("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (_appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime.TimeOfDay, endTime, id))
            {
                throw new Exception("El cliente ya tiene un turno asignado en ese horario.");
            }

            existing.StaffId = request.StaffId;
            existing.ClientId = clientId;
            existing.ServiceId = request.ServiceId;
            existing.Day = request.Day;
            existing.StartTime = request.StartTime.TimeOfDay;
            existing.EndTime = endTime;
            existing.Observation = request.Observation;
            existing.Payment = request.Payment;

            _appointmentRepository.Update(existing);

            var fullyLoaded = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Error al recargar el turno actualizado.");

            return fullyLoaded.ToResponse();
        }

        public AppointmentResponse UpdateStatus(Guid id, GestionTurnos.Domain.Entities.AppointmentStatus newStatus)
        {
            var existing = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Turno no encontrado.");

            existing.Status = newStatus;
            
            _appointmentRepository.Update(existing);

            var fullyLoaded = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Error al recargar el turno actualizado.");

            return fullyLoaded.ToResponse();
        }

        public void DeleteAppointment(Guid id)
        {
            var existing = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Turno no encontrado.");
            _appointmentRepository.Delete(id);
        }
    }
}