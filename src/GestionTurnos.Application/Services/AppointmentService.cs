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
        private readonly IStaffRepository _staffRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITenantProvider _tenantProvider;
        private readonly IClientService _clientService;
        private readonly IAppointmentNotificationService _appointmentNotificationService;

        public AppointmentService(IAppointmentRepository appointmentRepository, IClientService clientService, IStaffRepository staffRepository, IScheduleRepository scheduleRepository, ITenantProvider tenantProvider, IAppointmentNotificationService appointmentNotificationService)
        {
            _appointmentRepository = appointmentRepository;
            _staffRepository = staffRepository;
            _scheduleRepository = scheduleRepository;
            _tenantProvider = tenantProvider;
            _clientService = clientService;
            _appointmentNotificationService = appointmentNotificationService;
        }
        /// Valida que el turno caiga dentro del horario de atencion de la sucursal
        /// y devuelve el endTime calculado a partir de la duración del servicio.
        private TimeSpan ValidateAppointmentWithinSchedule(Guid branchId, DateTime day, TimeSpan startTime, int serviceDurationMinutes)
        {
            var dayOfWeek = day.DayOfWeek;

            var schedule = _scheduleRepository.GetByBranchIdAndDay(branchId, dayOfWeek)
                ?? throw new ConflictException("La sucursal no atiende el día seleccionado.");

            var endTime = startTime.Add(TimeSpan.FromMinutes(serviceDurationMinutes));

            if (startTime < schedule.StartTime || endTime > schedule.EndTime)
            {
                throw new ConflictException($"El horario del turno está fuera del horario de atención de la sucursal ({schedule.StartTime:hh\\:mm} - {schedule.EndTime:hh\\:mm}).");
            }

            return endTime;
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

            if(service.BusinessId != staff.BusinessId)
            {
                throw new ConflictException("El servicio no pertenece al negocio");
            }

            if (service.IsDeleted)
            {
                throw new ConflictException("El servicio no se encuentra disponible");
            }

            if(request.Day.Date < DateTime.Today)
            {
                throw new ConflictException("No se puede reservar turnos con fechas pasadas");
            }





            // 3. Busco o creo el cliente delegando a ClientService
            var clientDto = new ClientRequest
            {
                Name = request.ClientName,
                Email = request.ClientEmail,
                Phone = request.ClientPhone,
                BirthDay = request.ClientBirthDay.ToString("yyyy-MM-dd")
            };

            var clientResponse = _clientService.CreateClient(clientDto, staff.BusinessId);
            var clientId = clientResponse.Id;

            // 4. Valido que el turno caiga dentro del horario de la sucursal y calculo endTime
            var endTime = ValidateAppointmentWithinSchedule(request.BranchId, request.Day, request.StartTime, service.Duration);

            if (_appointmentRepository.ExistsOverlappingAppointment(request.StaffId, request.Day, request.StartTime, endTime))
            {
                throw new Exception("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (_appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime, endTime))
            {
                throw new Exception("El cliente ya tiene un turno asignado en ese horario.");
            }

            // 5. Crear el turno usando el precio real del servicio y el horario final calculado
            var appointment = request.ToEntity(clientId, service.Price, endTime);
            var appointmentCreated = _appointmentRepository.Add(appointment);

            var fullyLoaded = _appointmentRepository.GetById(appointmentCreated.Id) 
                ?? throw new Exception("Error al cargar el turno creado.");

            //ACA se manda el email para avisar TURNO
            Task.Run(() => _appointmentNotificationService.SendAppointmentConfirmationAsync(request, staff.Business.Name,
                staff.Branch.Address));

            //
            return fullyLoaded.ToResponse();
        }

        public AppointmentResponse UpdateAppointment(Guid id, AppointmentRequest request)
        {
            var existing = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Turno no encontrado.");

            // Obtener el Staff para derivar el BusinessId
            var staff = _staffRepository.GetById(request.StaffId)
                ?? throw new Exception("El profesional no fue encontrado.");

            // Resolver el cliente por email (find or create) delegando a ClientService
            var clientDto = new ClientRequest
            {
                Name = request.ClientName,
                Email = request.ClientEmail,
                Phone = request.ClientPhone,
                BirthDay = request.ClientBirthDay.ToString("yyyy-MM-dd")
            };

            var clientResponse = _clientService.CreateClient(clientDto, staff.BusinessId);
            var clientId = clientResponse.Id;

            // Obtener el servicio para sacar su duración
            var service = _appointmentRepository.GetServiceById(request.ServiceId)
                ?? throw new Exception("El servicio no fue encontrado.");

            var endTime = ValidateAppointmentWithinSchedule(request.BranchId, request.Day, request.StartTime, service.Duration);

            if (_appointmentRepository.ExistsOverlappingAppointment(request.StaffId, request.Day, request.StartTime, endTime, id))
            {
                throw new Exception("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (_appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime, endTime, id))
            {
                throw new Exception("El cliente ya tiene un turno asignado en ese horario.");
            }

            existing.StaffId = request.StaffId;
            existing.ClientId = clientId;
            existing.ServiceId = request.ServiceId;
            existing.Day = request.Day;
            existing.StartTime = request.StartTime;
            existing.EndTime = endTime;
            existing.Observation = request.Observation;
            existing.Payment = request.Payment;

            _appointmentRepository.Update(existing);

            var fullyLoaded = _appointmentRepository.GetById(id) 
                ?? throw new Exception("Error al recargar el turno actualizado.");

            return fullyLoaded.ToResponse();
        }

        public AppointmentResponse UpdateStatus(Guid id, AppointmentStatus newStatus)
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
