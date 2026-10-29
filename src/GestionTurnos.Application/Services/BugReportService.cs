using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Services
{
    public class BugReportService : IBugReportService
    {
        private readonly IBugReportRepository _bugReportRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly ITenantProvider _tenantProvider;

        public BugReportService(
            IBugReportRepository bugReportRepository,
            IBusinessRepository businessRepository,
            ITenantProvider tenantProvider)
        {
            _bugReportRepository = bugReportRepository;
            _businessRepository = businessRepository;
            _tenantProvider = tenantProvider;
        }

        // Los datos de quien reporta salen del JWT, nunca del body que manda
        // el cliente (ver pedido original: evitar que se puedan falsear).
        public async Task<BugReportResponse> Create(BugReportCreateRequest request)
        {
            var businessId = _tenantProvider.GetBusinessId();
            var bugReport = request.ToEntity(
                businessId,
                _tenantProvider.GetUserId(),
                _tenantProvider.GetUserName());

            await _bugReportRepository.Add(bugReport);

            // Si quien reporta es SysAdmin no tiene negocio asociado (businessId null).
            var business = businessId.HasValue
                ? await _businessRepository.GetById(businessId.Value)
                : null;

            return bugReport.ToResponse(business);
        }

        // Para que el SysAdmin vea de que negocio vino cada reporte (nombre,
        // slug, etc.) en vez de un GUID crudo. Se resuelve en memoria contra
        // todos los negocios en vez de ir uno por uno, ya que GetById no tiene
        // una variante "por lista de ids" en el repositorio.
        public async Task<List<BugReportResponse>> GetAll()
        {
            var bugReports = await _bugReportRepository.GetAllGlobal();
            var businesses = (await _businessRepository.GetAllGlobal())
                .ToDictionary(b => b.Id);

            return bugReports
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => b.ToResponse(
                    b.BusinessId.HasValue && businesses.TryGetValue(b.BusinessId.Value, out var business)
                        ? business
                        : null))
                .ToList();
        }

        public async Task<BugReportResponse> UpdateStatus(Guid id, BugReportStatusUpdateRequest request)
        {
            var bugReport = await _bugReportRepository.GetById(id)
                ?? throw new NotFoundException("No se encontró el reporte.");

            bugReport.Status = request.Status;
            await _bugReportRepository.Update(bugReport);

            var business = bugReport.BusinessId.HasValue
                ? await _businessRepository.GetById(bugReport.BusinessId.Value)
                : null;

            return bugReport.ToResponse(business);
        }
    }
}
