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
        private readonly IAppointmentRealtimeNotifier _appointmentRealtimeNotifier;

        public AppointmentService(IAppointmentRepository appointmentRepository, IClientService clientService, IStaffRepository staffRepository, IScheduleRepository scheduleRepository, ITenantProvider tenantProvider, IAppointmentNotificationService appointmentNotificationService, IAppointmentRealtimeNotifier appointmentRealtimeNotifier)
        {
            _appointmentRepository = appointmentRepository;
            _staffRepository = staffRepository;
            _scheduleRepository = scheduleRepository;
            _tenantProvider = tenantProvider;
            _clientService = clientService;
            _appointmentNotificationService = appointmentNotificationService;
            _appointmentRealtimeNotifier = appointmentRealtimeNotifier;
        }
        /// Valida que el turno caiga dentro del horario de atencion de la sucursal
        /// y devuelve el endTime calculado a partir de la duracion del servicio.
        private async Task<TimeSpan> ValidateAppointmentWithinSchedule(Guid branchId, DateTime day, TimeSpan startTime, int serviceDurationMinutes)
        {
            var dayOfWeek = day.DayOfWeek;

            var schedule = await _scheduleRepository.GetByBranchIdAndDay(branchId, dayOfWeek)
                ?? throw new ConflictException("La sucursal no atiende el día seleccionado.");

            var endTime = startTime.Add(TimeSpan.FromMinutes(serviceDurationMinutes));

            if (startTime < schedule.StartTime || endTime > schedule.EndTime)
            {
                throw new ConflictException($"El horario del turno está fuera del horario de atención de la sucursal ({schedule.StartTime:hh\\:mm} - {schedule.EndTime:hh\\:mm}).");
            }

            return endTime;
        }

        public async Task<List<GlobalAppointmentResponse>> GetAllGlobal()
        {
            var appointments = await _appointmentRepository.GetAllGlobal();

            return appointments
                .Select(a => a.ToGlobalResponse())
                .ToList();
        }

        public async Task<List<AppointmentResponse>> GetAppointmentsOfCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var appointments = await _appointmentRepository.GetByBusinessId(businessId);
            return appointments
                .Select(a => a.ToResponse())
                .ToList();
        }

        public async Task<List<AppointmentResponse>> GetAppointmentsOfMyBranch()
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
                var staffAppointments = await _appointmentRepository.GetByStaffId(userId, businessId);
                return staffAppointments
                    .Select(a => a.ToResponse())
                    .ToList();
            }

            // Para Recepcionista o Admin, traemos todos los de la sucursal
            var branchAppointments = await _appointmentRepository.GetByBranchId(branchId, businessId);
            return branchAppointments
                .Select(a => a.ToResponse())
                .ToList();
        }

        public async Task<List<AppointmentResponse>> GetMyAppointments()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var userId = _tenantProvider.GetUserId()
                ?? throw new ConflictException("No se encontró el id del usuario.");

            var appointments = await _appointmentRepository.GetByStaffId(userId, businessId);
            return appointments
                .Select(a => a.ToResponse())
                .ToList();
        }

        public async Task<List<AppointmentResponse>> GetAppointmentsByBranch(Guid branchId)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var appointments = await _appointmentRepository.GetByBranchId(branchId, businessId);
            return appointments
                .Select(a => a.ToResponse())
                .ToList();
        }

        public async Task<List<AppointmentResponse>> GetAppointmentsByBranchAndDate(DateTime day, Guid? branchId = null)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var appointments = await _appointmentRepository.GetByBranchIdAndDay(businessId, day, branchId);
            return appointments
                .Select(a => a.ToResponse())
                .ToList();
        }

        public async Task<AppointmentResponse> GetById(Guid id)
        {
            var appointment = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Turno no encontrado.");
            return appointment.ToResponse();
        }

        public async Task<AppointmentResponse> CreateAppointment(AppointmentRequest request)
        {
            // 1. Obtener el Staff para derivar el BusinessId
            var staff = await _staffRepository.GetById(request.StaffId)
                ?? throw new Exception("El profesional no fue encontrado.");

            // 2. Validar que el staff pertenece a la sucursal indicada
            if (staff.BranchId != request.BranchId)
                throw new ConflictException("El profesional seleccionado no pertenece a esta sucursal.");

            // 3. Obtener el servicio para calcular el costo real
            var service = await _appointmentRepository.GetServiceById(request.ServiceId)
                ?? throw new Exception("El servicio no fue encontrado.");

            if(service.BusinessId != staff.BusinessId)
            {
                throw new ConflictException("El servicio no pertenece al negocio");
            }

            if (service.IsDeleted)
            {
                throw new ConflictException("El servicio no se encuentra disponible");
            }

            var argDate = DateTime.UtcNow.AddHours(-3).Date;
            if(request.Day.Date < argDate)
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

            var clientResponse = await _clientService.CreateClient(clientDto, staff.BusinessId);
            var clientId = clientResponse.Id;

            // 4. Valido que el turno caiga dentro del horario de la sucursal y calculo endTime
            var endTime = await ValidateAppointmentWithinSchedule(request.BranchId, request.Day, request.StartTime, service.Duration);

            if (await _appointmentRepository.ExistsOverlappingAppointment(request.StaffId, request.Day, request.StartTime, endTime))
            {
                throw new Exception("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (await _appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime, endTime))
            {
                throw new Exception("El cliente ya tiene un turno asignado en ese horario.");
            }

            // 5. Crear el turno usando el precio real del servicio y el horario final calculado
            var appointment = request.ToEntity(clientId, service.Price, endTime);
            var appointmentCreated = await _appointmentRepository.Add(appointment);

            var fullyLoaded = await _appointmentRepository.GetById(appointmentCreated.Id)
                ?? throw new Exception("Error al cargar el turno creado.");

            await _appointmentRealtimeNotifier.NotifyAppointmentCreatedAsync(fullyLoaded.ToNotificationPayload());

            //ACA se manda el email para avisar TURNO
            await _appointmentNotificationService.SendAppointmentConfirmationAsync(request, staff.Business.Name,
                staff.Branch.Address);

            //
            return fullyLoaded.ToResponse();
        }

        public async Task<AppointmentResponse> UpdateAppointment(Guid id, AppointmentRequest request)
        {
            var existing = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Turno no encontrado.");

            // Obtener el Staff para derivar el BusinessId
            var staff = await _staffRepository.GetById(request.StaffId)
                ?? throw new Exception("El profesional no fue encontrado.");

            // Resolver el cliente por email (find or create) delegando a ClientService
            var clientDto = new ClientRequest
            {
                Name = request.ClientName,
                Email = request.ClientEmail,
                Phone = request.ClientPhone,
                BirthDay = request.ClientBirthDay.ToString("yyyy-MM-dd")
            };

            var clientResponse = await _clientService.CreateClient(clientDto, staff.BusinessId);
            var clientId = clientResponse.Id;

            // Obtener el servicio para sacar su duración
            var service = await _appointmentRepository.GetServiceById(request.ServiceId)
                ?? throw new Exception("El servicio no fue encontrado.");

            var endTime = await ValidateAppointmentWithinSchedule(request.BranchId, request.Day, request.StartTime, service.Duration);

            if (await _appointmentRepository.ExistsOverlappingAppointment(request.StaffId, request.Day, request.StartTime, endTime, id))
            {
                throw new Exception("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (await _appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime, endTime, id))
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

            await _appointmentRepository.Update(existing);

            var fullyLoaded = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Error al recargar el turno actualizado.");

            return fullyLoaded.ToResponse();
        }

        public async Task<AppointmentResponse> UpdateStatus(Guid id, AppointmentStatus newStatus)
        {
            var existing = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Turno no encontrado.");

            var wasNotCancelled = existing.Status != AppointmentStatus.Cancelled;

            existing.Status = newStatus;

            await _appointmentRepository.Update(existing);

            var fullyLoaded = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Error al recargar el turno actualizado.");

            if (newStatus == AppointmentStatus.Cancelled &&
                wasNotCancelled &&
                fullyLoaded.Day.Date >= DateTime.UtcNow.AddHours(-3).Date)
            {
                await _appointmentNotificationService.SendAppointmentCancelledAsync(fullyLoaded);
            }

            return fullyLoaded.ToResponse();
        }

        public async Task DeleteAppointment(Guid id)
        {
            var existing = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Turno no encontrado.");
            await _appointmentRepository.Delete(id);
        }

        public async Task<List<AvailableSlotResponse>> GetAvailableSlots(Guid branchId, Guid staffId, Guid serviceId, DateTime date)
        {
            var staff = await _staffRepository.GetById(staffId);
            if (staff == null || staff.BranchId != branchId)
            {
                return new List<AvailableSlotResponse>();
            }

            var service = await _appointmentRepository.GetServiceById(serviceId);
            if (service == null || service.IsDeleted || service.BusinessId != staff.BusinessId)
            {
                return new List<AvailableSlotResponse>();
            }

            var schedule = await _scheduleRepository.GetByBranchIdAndDay(branchId, date.DayOfWeek);
            if (schedule == null)
            {
                return new List<AvailableSlotResponse>();
            }

            var serviceDuration = TimeSpan.FromMinutes(service.Duration);
            var slotStep = TimeSpan.FromMinutes(schedule.SlotDurationMinutes);

            if (slotStep <= TimeSpan.Zero || serviceDuration <= TimeSpan.Zero)
            {
                return new List<AvailableSlotResponse>();
            }

            var existingAppointments = await _appointmentRepository.GetByStaffIdAndDay(staffId, date);

            var result = new List<AvailableSlotResponse>();

            for (var candidateStart = schedule.StartTime;
                 candidateStart + serviceDuration <= schedule.EndTime;
                 candidateStart += slotStep)
            {
                var candidateEnd = candidateStart + serviceDuration;

                bool overlaps = existingAppointments.Any(a =>
                    a.StartTime < candidateEnd && a.EndTime > candidateStart);

                if (!overlaps)
                {
                    result.Add(new AvailableSlotResponse
                    {
                        StartTime = candidateStart.ToString(@"hh\:mm"),
                        EndTime = candidateEnd.ToString(@"hh\:mm")
                    });
                }
            }

            return result;
        }
    }
}
