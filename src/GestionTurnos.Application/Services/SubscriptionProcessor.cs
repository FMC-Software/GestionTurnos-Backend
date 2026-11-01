using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

public class SubscriptionProcessor
{
    private readonly IBusinessSubscriptionRepository _subRepo;
    private readonly IEmailContentBuilder _emailBuilder;
    private readonly IEmailService _emailService;
    private readonly IStaffRepository _staffRepo;
    private readonly IPlanService _planService;
    private readonly IStaffLimitEnforcer _staffLimitEnforcer;

    private readonly IServiceLimitEnforcer _serviceLimitEnforcer;
    private readonly IBranchLimitEnforcer _branchLimitEnforcer;

    public SubscriptionProcessor(IBusinessSubscriptionRepository subRepo,IStaffRepository staffRepo,IEmailContentBuilder emailBuilder,IEmailService emailService,IPlanService planService,IStaffLimitEnforcer staffLimitEnforcer,IServiceLimitEnforcer serviceLimitEnforcer,IBranchLimitEnforcer branchLimitEnforcer)
    {
        _serviceLimitEnforcer = serviceLimitEnforcer;
        _branchLimitEnforcer = branchLimitEnforcer;
        _subRepo = subRepo;
        _staffRepo = staffRepo;
        _emailBuilder = emailBuilder;
        _emailService = emailService;
        _planService = planService;
        _staffLimitEnforcer = staffLimitEnforcer;
    }

    public async Task ExecuteAsync()
    {
        var activeSubs = await _subRepo.GetActiveSubscriptionsAsync();


        var allStaff = await _staffRepo.GetAllGlobal();

        foreach (var sub in activeSubs)
        {
            var businessAdmin = allStaff
                .FirstOrDefault(s => s.BusinessId == sub.Business.Id && s.Rol == Rol.Admin);

            var now = DateTime.UtcNow;
            var isFree = sub.Plan.Price <= 0;

            // Aviso de proximo vencimiento (el Free Plan no vence, se renueva solo).
            if (!isFree && businessAdmin != null && sub.EndDate <= now.AddDays(3) && sub.EndDate > now)
            {
                var email = _emailBuilder.BuildVencimientoEmail(businessAdmin.Email, sub.Business.Name, 3);
                await _emailService.SendEmailAsync(email);
            }

            if (sub.EndDate > now)
                continue;

            // Plan pendiente (downgrade programado), si sigue vigente.
            Plan? pendingPlan = null;
            if (sub.PendingPlanId.HasValue)
            {
                try { pendingPlan = await _planService.GetActivePlan(sub.PendingPlanId.Value); }
                catch { pendingPlan = null; }
            }

            // Free Plan sin cambio pendiente: se renueva en el lugar, sin vencer.
            if (isFree && pendingPlan == null)
            {
                sub.StartDate = now;
                sub.EndDate = now.AddDays(sub.Plan.DurationDays);
                sub.PendingPlanId = null;
                await _subRepo.UpdateAsync(sub);
                continue;
            }

            sub.Status = Status.Expired;
            await _subRepo.UpdateAsync(sub);

            // Al vencer, el negocio pasa al plan pendiente o, si no hay, al plan gratuito,
            // y se aplican los limites de ese plan.
            var targetPlan = pendingPlan ?? await _planService.GetPlanOrDefault(null);
            await _subRepo.Add(new BusinessSubscription
            {
                BusinessId = sub.BusinessId,
                PlanId = targetPlan.Id,
                StartDate = now,
                EndDate = now.AddDays(targetPlan.DurationDays),
                Status = Status.Active
            });
            await _staffLimitEnforcer.ApplyStaffLimitAsync(sub.BusinessId, targetPlan);
            await _serviceLimitEnforcer.ApplyServiceLimitAsync(sub.BusinessId, targetPlan);
            await _branchLimitEnforcer.ApplyBranchLimitAsync(sub.BusinessId, targetPlan);

            if (pendingPlan == null && businessAdmin != null)
            {
                var email = _emailBuilder.BuildExpiredEmail(businessAdmin.Email, sub.Business.Name);
                await _emailService.SendEmailAsync(email);
            }
        }
    }
}