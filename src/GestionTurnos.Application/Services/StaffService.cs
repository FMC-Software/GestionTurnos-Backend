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

        public StaffsResponse CreateStaff(StaffRequest request)
        {
            var existingStaff = _staffRepository.GetAll().FirstOrDefault(s => s.Email == request.Email);
            if (existingStaff != null)
            {
                throw new ConflictException("Ya existe un usuario con ese correo electrónico.");
            }
            var AdminExisting = _staffRepository.GetAll().Any(s => s.Rol == request.Rol); 
            if (AdminExisting == false)
            {
                throw new ConflictException("Cada negocio solo puede tener un Admin ");
            }
            var IdBusiness = _tenantProvider.GetBusinessId()
                ?? Guid.Empty;
            var newStaff = request.ToStaff();
            
            newStaff.BusinessId = IdBusiness;
            _staffRepository.Add(newStaff);

            return newStaff.ToResponse();
        }

        public List<StaffsResponse> GetStaffOfCurrentBusiness()
        {
            
            var staffList = _staffRepository.GetAll().Where(s => s.Rol != Rol.Admin);
            return staffList.Select(s => s.ToResponse()).ToList();
        }

        public StaffsResponse GetById(Guid id)
        {
            var staff = _staffRepository.GetById(id)
                ?? throw new KeyNotFoundException("Usuario no encontrado o no pertenece a su comercio.");
            return staff.ToResponse();
        }

        public StaffsResponse UpdateStaff(StaffRequest request, Guid idStaff)
        {
            var existingStaff = _staffRepository.GetById(idStaff)
                ?? throw new ConflictException("Usuario no encontrado.");

           
            existingStaff.UpdateFromDto(request);

            _staffRepository.Update(existingStaff);
            return existingStaff.ToResponse();
        }

        public void DeleteStaff(Guid id)
        {
            var staff = _staffRepository.GetById(id)
                ?? throw new ConflictException("Usuario no encontrado.");
            _staffRepository.Delete(id);
        }

        public List<GlobalStaffResponse> GetAllGlobal()
        {
            var globalList = _staffRepository.GetAllGlobal();
            return globalList.Select(s => s.ToGlobalResponse()).ToList();
        }

        public Staff GetByEmail(string email)
        {
            var staff = _staffRepository.GetByEmail(email) ?? null;
            
            return staff;
        }
    }
}