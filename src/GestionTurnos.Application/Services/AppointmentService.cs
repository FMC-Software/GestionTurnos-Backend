using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
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
        private readonly ITenantProvider _tenantProvider;

        public AppointmentService(IAppointmentRepository appointmentRepository, IClientRepository clientRepository, IStaffRepository staffRepository, ITenantProvider tenantProvider)
        {
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
            _tenantProvider = tenantProvider;
        }

        public List<GlobalAppointmentResponse> GetAllGlobal()
        {
            var appointments = _appointmentRepository.GetAllGlobal();

            return appointments
                .Select(a => a.ToGlobalResponse())
                .ToList();
        }

        public List<AppointmentResponse> GetAppointmentsOfCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            return _appointmentRepository.GetByBusinessId(businessId)
                .Select(a => a.ToResponse())
                .ToList();
        }

        public List<AppointmentResponse> GetAppointmentsOfMyBranch()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");
            
            var branchId = _tenantProvider.GetBranchId()
                ?? throw new ConflictException("No se encontró la sucursal asignada al usuario.");
                
            var role = _tenantProvider.GetUserRole()
                ?? throw new ConflictException("No se encontró el rol del usuario.");

            var userId = _tenantProvider.GetUserId()
                ?? throw new ConflictException("No se encontró el id del usuario.");

            if (Enum.TryParse(role, out Rol userRole) && userRole == Rol.Profesional)
            {
                return _appointmentRepository.GetByStaffId(userId, businessId)
                    .Select(a => a.ToResponse())
                    .ToList();
            }

            // Para Recepcionista o Admin, traemos todos los de la sucursal
            return _appointmentRepository.GetByBranchId(branchId, businessId)
                .Select(a => a.ToResponse())
                .ToList();
        }

        public List<AppointmentResponse> GetMyAppointments()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");
            
            var userId = _tenantProvider.GetUserId()
                ?? throw new ConflictException("No se encontró el id del usuario.");

            return _appointmentRepository.GetByStaffId(userId, businessId)
                .Select(a => a.ToResponse())
                .ToList();
        }

        public List<AppointmentResponse> GetAppointmentsByBranch(Guid branchId)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            return _appointmentRepository.GetByBranchId(branchId, businessId)
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

            // 2. Validar que el staff pertenece a la sucursal indicada
            if (staff.BranchId != request.BranchId)
                throw new ConflictException("El profesional seleccionado no pertenece a esta sucursal.");

            // 3. Obtener el servicio para calcular el costo real
            var service = _appointmentRepository.GetServiceById(request.ServiceId)
                ?? throw new Exception("El servicio no fue encontrado.");

            // 3. Busco cliente por email. Si no existe, se crea.
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

            // 4. Verifico solapamiento de horarios
            var endTime = request.StartTime.TimeOfDay.Add(TimeSpan.FromHours(1));

            if (_appointmentRepository.ExistsOverlappingAppointment(request.StaffId, request.Day, request.StartTime.TimeOfDay, endTime))
            {
                throw new Exception("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (_appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime.TimeOfDay, endTime))
            {
                throw new Exception("El cliente ya tiene un turno asignado en ese horario.");
            }

            // 5. Crear el turno usando el precio real del servicio
            var appointment = request.ToEntity(clientId, service.Price);
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