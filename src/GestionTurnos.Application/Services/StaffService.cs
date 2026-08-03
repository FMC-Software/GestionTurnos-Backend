using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ITenantProvider _tenantProvider;

        public StaffService(IStaffRepository staffRepository, ITenantProvider tenantProvider)
        {
            _staffRepository = staffRepository;
            _tenantProvider = tenantProvider;
        }

        public async Task<StaffsResponse> CreateStaff(StaffRequest request)
        {
            var existingStaff = await _staffRepository.GetByEmail(request.Email);
            if (existingStaff != null)
            {
                throw new ConflictException("Ya existe un usuario con ese correo electrónico.");
            }
            var staffList = await _staffRepository.GetAll();
            var AdminExisting = staffList.Any(s => s.Rol == Rol.Admin && request.Rol == Rol.Admin); //No anda arreglalo
            if (AdminExisting)
            {
                var adminExisting = staffList.Any(s => s.Rol == Rol.Admin);
                if (adminExisting)
                    throw new ConflictException("Cada negocio solo puede tener un Admin.");
            }
            var IdBusiness = _tenantProvider.GetBusinessId()
                ?? Guid.Empty;
            var newStaff = request.ToStaff();

            newStaff.BusinessId = IdBusiness;
            await _staffRepository.Add(newStaff);

            return newStaff.ToResponse();
        }

        public async Task<List<StaffsResponse>> GetStaffOfCurrentBusiness()
        {

            var staffList = (await _staffRepository.GetAll()).Where(s => s.Rol != Rol.Admin);
            return staffList.Select(s => s.ToResponse()).ToList();
        }

        public async Task<StaffsResponse> GetById(Guid id)
        {
            var staff = await _staffRepository.GetById(id)
                ?? throw new KeyNotFoundException("Usuario no encontrado o no pertenece a su comercio.");
            return staff.ToResponse();
        }

        public async Task<StaffsResponse> UpdateStaff(StaffRequest request, Guid idStaff)
        {
            var existingStaff = await _staffRepository.GetById(idStaff)
                ?? throw new ConflictException("Usuario no encontrado.");


            existingStaff.UpdateFromDto(request);

            await _staffRepository.Update(existingStaff);
            return existingStaff.ToResponse();
        }

        public async Task DeleteStaff(Guid id)
        {
            var staff = await _staffRepository.GetById(id)
                ?? throw new ConflictException("Usuario no encontrado.");
            await _staffRepository.Delete(id);
        }

        public async Task<List<GlobalStaffResponse>> GetAllGlobal()
        {
            var globalList = await _staffRepository.GetAllGlobal();
            return globalList.Select(s => s.ToGlobalResponse()).ToList();
        }

        public async Task<Staff?> GetByEmail(string email)
        {
            var staff = await _staffRepository.GetByEmail(email) ?? null;

            return staff;
        }

        public async Task<Staff?> GetByEmailGlobal(string email)
        {
            var staff = await _staffRepository.GetByEmailGlobal(email) ?? null;

            return staff;
        }
    }
}
