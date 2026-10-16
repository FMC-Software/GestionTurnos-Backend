using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Services
{
    public class SysAdminService : ISysAdminService
    {
        private readonly ISysAdminRepository _sysAdminRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILandingContentService _landingContentService;

        public SysAdminService(
            ISysAdminRepository sysAdminRepository,
            IBusinessRepository businessRepository,
            IBranchRepository branchRepository,
            IStaffRepository staffRepository,
            IBusinessSubscriptionRepository subscriptionRepository,
            ITenantProvider tenantProvider,
            ILandingContentService landingContentService)
        {
            _sysAdminRepository = sysAdminRepository;
            _businessRepository = businessRepository;
            _branchRepository = branchRepository;
            _staffRepository = staffRepository;
            _subscriptionRepository = subscriptionRepository;
            _tenantProvider = tenantProvider;
            _landingContentService = landingContentService;
        }

        public async Task<SysAdminUser?> GetByEmail(string email)
        {
            var sysAdmin = await _sysAdminRepository.GetByEmail(email) ?? null;

            return sysAdmin;
        }

        public async Task<List<BusinessCardResponse>> GetBusinessCards()
        {
            return await _businessRepository.GetBusinessCardsAsync();
        }

        public async Task<BusinessDetailResponse> GetBusinessDetail(Guid businessId)
        {
            var business = await _businessRepository.GetById(businessId)
                ?? throw new NotFoundException("El negocio especificado no existe.");

            var stats = await _businessRepository.GetBusinessStatsAsync(businessId) ?? new BusinessStatsResult();
            var branches = await _branchRepository.GetByBusinessId(businessId);
            var admin = await _staffRepository.GetAdminByBusinessId(businessId);
            var subscription = await _subscriptionRepository.GetCurrentSubscription(businessId)
                ?? await _subscriptionRepository.GetLatestByBusinessId(businessId);

            return new BusinessDetailResponse
            {
                Id = business.Id,
                Name = business.Name,
                Email = admin?.Email,
                Phone = branches.FirstOrDefault(b => !string.IsNullOrWhiteSpace(b.Phone))?.Phone,
                Url = business.Url,
                UrlLogo = business.UrlLogo,
                TypeBusiness = business.TypeBusiness,
                Status = business.IsActive,
                UpdateDateTime = business.UpdateDateTime,
                CurrentPlanId = subscription?.PlanId,
                CurrentPlan = subscription?.Plan?.Name,
                SubscriptionStatus = subscription?.Status.ToString(),
                SubscriptionStartDate = subscription?.StartDate,
                SubscriptionEndDate = subscription?.EndDate,
                BranchCount = stats.BranchCount,
                StaffCount = stats.StaffCount,
                ClientCount = stats.ClientCount,
                AppointmentCount = stats.AppointmentCount,
                Branches = branches.Select(b => b.ToBranchDetailResponse()).ToList()
            };
        }

        private async Task<SysAdminUser> GetCurrentSysAdmin()
        {
            var userId = _tenantProvider.GetUserId()
                ?? throw new ConflictException("No se encontró el usuario en el token.");

            return await _sysAdminRepository.GetById(userId)
                ?? throw new NotFoundException("Usuario no encontrado.");
        }

        public async Task<SysAdminProfileResponse> GetMyProfile()
        {
            var sysAdmin = await GetCurrentSysAdmin();
            var landing = await _landingContentService.Get();
            return new SysAdminProfileResponse
            {
                Id = sysAdmin.Id,
                Name = sysAdmin.Name,
                Email = sysAdmin.Email,
                Phone = sysAdmin.Phone,
                LinkPhoto = landing.LinkPhoto
            };
        }

        public async Task<SysAdminProfileResponse> UpdateMyProfile(UpdateSysAdminProfileRequest request)
        {
            var sysAdmin = await GetCurrentSysAdmin();

            if (!string.IsNullOrWhiteSpace(request.Name))
                sysAdmin.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                sysAdmin.Phone = request.Phone;

            await _sysAdminRepository.Update(sysAdmin);

            if (!string.IsNullOrWhiteSpace(request.LinkPhoto))
                await _landingContentService.UpdateLinkPhoto(request.LinkPhoto);

            var landing = await _landingContentService.Get();
            return new SysAdminProfileResponse
            {
                Id = sysAdmin.Id,
                Name = sysAdmin.Name,
                Email = sysAdmin.Email,
                Phone = sysAdmin.Phone,
                LinkPhoto = landing.LinkPhoto
            };
        }
    }
}
