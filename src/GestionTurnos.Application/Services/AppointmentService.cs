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
        private readonly IBranchRepository _branchRepository;
        private readonly ITenantProvider _tenantProvider;
        private readonly IClientService _clientService;
        private readonly IAppointmentNotificationService _appointmentNotificationService;
        private readonly IAppointmentRealtimeNotifier _appointmentRealtimeNotifier;

        public AppointmentService(IAppointmentRepository appointmentRepository, IClientService clientService, IStaffRepository staffRepository, IScheduleRepository scheduleRepository, IBranchRepository branchRepository, ITenantProvider tenantProvider, IAppointmentNotificationService appointmentNotificationService, IAppointmentRealtimeNotifier appointmentRealtimeNotifier)
        {
            _appointmentRepository = appointmentRepository;
            _staffRepository = staffRepository;
            _scheduleRepository = scheduleRepository;
            _branchRepository = branchRepository;
            _tenantProvider = tenantProvider;
            _clientService = clientService;
            _appointmentNotificationService = appointmentNotificationService;
            _appointmentRealtimeNotifier = appointmentRealtimeNotifier;
        }
        private const int MaxAppointmentMonthsAhead = 2;

        /// Valida que la fecha del turno no supere el maximo de meses de anticipacion permitido.
        private static void ValidateAppointmentDateRange(DateTime day, DateTime nowArgentina)
        {
            var maxDate = nowArgentina.Date.AddMonths(MaxAppointmentMonthsAhead);
            if (day.Date > maxDate)
            {
                throw new ConflictException($"No se puede reservar turnos con más de {MaxAppointmentMonthsAhead} meses de anticipación.");
            }
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

        public async Task<BusinessAppointmentStatsResponse> GetAppointmentStatsForCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            return new BusinessAppointmentStatsResponse
            {
                TotalAppointments = await _appointmentRepository.CountByBusinessId(businessId),
                ActiveClients = await _appointmentRepository.CountDistinctActiveClientsByBusinessId(businessId),
            };
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

        public async Task<List<AppointmentResponse>> GetAppointmentsOfMyBranchByDate(DateTime day)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var branchId = _tenantProvider.GetBranchId()
                ?? throw new ConflictException("No se encontró la sucursal asignada al usuario.");

            var appointments = await _appointmentRepository.GetByBranchIdAndDay(businessId, day, branchId);
            return appointments
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

            if (!staff.IsActive)
                throw new ConflictException("El profesional seleccionado no está disponible.");

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

            var nowArgentina = DateTime.UtcNow.AddHours(-3);
            if(request.Day.Date < nowArgentina.Date)
            {
                throw new ConflictException("No se puede reservar turnos con fechas pasadas");
            }

            if(request.Day.Date == nowArgentina.Date && request.StartTime <= nowArgentina.TimeOfDay)
            {
                throw new ConflictException("No se puede reservar un turno en un horario que ya pasó");
            }

            ValidateAppointmentDateRange(request.Day, nowArgentina);

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
                throw new ConflictException("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (await _appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime, endTime))
            {
                throw new ConflictException("El cliente ya tiene un turno asignado en ese horario.");
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

            var nowArgentina = DateTime.UtcNow.AddHours(-3);
            if(request.Day.Date < nowArgentina.Date)
            {
                throw new ConflictException("No se puede reservar turnos con fechas pasadas");
            }

            if(request.Day.Date == nowArgentina.Date && request.StartTime <= nowArgentina.TimeOfDay)
            {
                throw new ConflictException("No se puede reservar un turno en un horario que ya pasó");
            }

            ValidateAppointmentDateRange(request.Day, nowArgentina);

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
                throw new ConflictException("El profesional ya tiene un turno asignado en ese horario.");
            }

            if (await _appointmentRepository.ExistsOverlappingAppointmentForClient(clientId, request.Day, request.StartTime, endTime, id))
            {
                throw new ConflictException("El cliente ya tiene un turno asignado en ese horario.");
            }

            existing.StaffId = request.StaffId;
            existing.ClientId = clientId;
            existing.ClientName = request.ClientName;
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

        public async Task<List<AppointmentResponse>> GetPendingReassignments()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var appointments = await _appointmentRepository.GetPendingReassignmentByBusinessId(businessId);
            return appointments.Select(a => a.ToResponse()).ToList();
        }

        public async Task<AppointmentResponse> ReassignAppointment(Guid id, Guid newStaffId)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var existing = await _appointmentRepository.GetById(id)
                ?? throw new NotFoundException("Turno no encontrado.");

            if (existing.Staff.BusinessId != businessId)
                throw new NotFoundException("Turno no encontrado.");

            if (existing.Status != AppointmentStatus.PendingReassignment)
                throw new ConflictException("El turno no está pendiente de reasignación.");

            if (existing.Day.Date < DateTime.UtcNow.AddHours(-3).Date)
                throw new ConflictException("No se puede reasignar un turno con fecha pasada.");

            var newStaff = await _staffRepository.GetById(newStaffId)
                ?? throw new NotFoundException("El profesional no fue encontrado.");

            if (newStaff.BusinessId != businessId)
                throw new NotFoundException("El profesional no fue encontrado.");

            if (!newStaff.IsActive)
                throw new ConflictException("El profesional seleccionado no está activo.");

            if (newStaff.Id == existing.StaffId)
                throw new ConflictException("Debe seleccionar un profesional distinto al original.");

            if (newStaff.BranchId != existing.Staff.BranchId)
                throw new ConflictException("El profesional seleccionado no pertenece a la sucursal del turno.");

            var endTime = await ValidateAppointmentWithinSchedule(newStaff.BranchId, existing.Day, existing.StartTime, existing.Service.Duration);

            if (await _appointmentRepository.ExistsOverlappingAppointment(newStaff.Id, existing.Day, existing.StartTime, endTime, id))
                throw new ConflictException("El profesional ya tiene un turno asignado en ese horario.");

            existing.StaffId = newStaff.Id;
            existing.Staff = newStaff;
            existing.EndTime = endTime;
            existing.Status = AppointmentStatus.Confirmed;

            await _appointmentRepository.Update(existing);

            var fullyLoaded = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Error al recargar el turno actualizado.");

            return fullyLoaded.ToResponse();
        }

        public async Task<AppointmentResponse> UpdateStatus(Guid id, AppointmentStatus newStatus)
        {
            var existing = await _appointmentRepository.GetById(id)
                ?? throw new Exception("Turno no encontrado.");

            var role = _tenantProvider.GetUserRole();
            if (Enum.TryParse(role, out Rol userRole) && userRole == Rol.Profesional)
            {
                var userId = _tenantProvider.GetUserId();
                if (existing.StaffId != userId)
                {
                    throw new ConflictException("No puede modificar un turno que no le pertenece.");
                }
            }

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
            if (staff == null || !staff.IsActive || staff.BranchId != branchId)
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

            var nowArgentina = DateTime.UtcNow.AddHours(-3);
            var isToday = date.Date == nowArgentina.Date;

            var result = new List<AvailableSlotResponse>();

            for (var candidateStart = schedule.StartTime;
                 candidateStart + serviceDuration <= schedule.EndTime;
                 candidateStart += slotStep)
            {
                if (isToday && candidateStart <= nowArgentina.TimeOfDay)
                {
                    continue;
                }

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

        public async Task<BranchAgendaResponse> GetBranchAgenda(Guid branchId, DateTime date)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var branch = await _branchRepository.GetById(branchId)
                ?? throw new ConflictException("Sucursal no encontrada.");

            if (branch.BusinessId != businessId)
            {
                throw new ConflictException("La sucursal no pertenece a su negocio.");
            }

            var schedule = await _scheduleRepository.GetByBranchIdAndDay(branchId, date.DayOfWeek);
            var staffList = await _staffRepository.GetByBranchId(branchId);
            var appointments = await _appointmentRepository.GetByBranchIdAndDay(businessId, date, branchId);

            var appointmentsByStaff = appointments.ToLookup(a => a.StaffId);

            return new BranchAgendaResponse
            {
                BranchId = branch.Id,
                BranchName = branch.Name,
                Date = date.Date,
                Schedule = schedule == null ? null : new ScheduleInfoResponse
                {
                    StartTime = schedule.StartTime.ToString(@"hh\:mm"),
                    EndTime = schedule.EndTime.ToString(@"hh\:mm"),
                    SlotDurationMinutes = schedule.SlotDurationMinutes
                },
                Staff = staffList.Select(s => new StaffAgendaResponse
                {
                    StaffId = s.Id,
                    StaffName = s.Name,
                    Appointments = appointmentsByStaff[s.Id].Select(a => new AgendaAppointmentResponse
                    {
                        Id = a.Id,
                        StartTime = a.StartTime.ToString(@"hh\:mm"),
                        EndTime = a.EndTime.ToString(@"hh\:mm"),
                        ClientName = a.ClientName,
                        ServiceName = a.Service.Name,
                        Status = a.Status.ToString()
                    }).ToList()
                }).ToList()
            };
        }
    }
}
