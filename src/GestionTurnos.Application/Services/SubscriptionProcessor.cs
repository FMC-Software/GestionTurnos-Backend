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

    public SubscriptionProcessor(IBusinessSubscriptionRepository subRepo,IStaffRepository staffRepo,IEmailContentBuilder emailBuilder,IEmailService emailService,IPlanService planService,IStaffLimitEnforcer staffLimitEnforcer)
    {
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

            if (businessAdmin == null)
                continue; // salta esta suscripción, sigue con las demás

            if (sub.EndDate.AddDays(1) < DateTime.UtcNow)
                continue;

            if (sub.EndDate <= DateTime.UtcNow.AddDays(3) && sub.EndDate > DateTime.UtcNow )
            {
                var email = _emailBuilder.BuildVencimientoEmail(businessAdmin.Email, sub.Business.Name, 3);
                await _emailService.SendEmailAsync(email);
            }

            if (sub.EndDate <= DateTime.UtcNow)
            {
                sub.Status = Status.Expired;
                await _subRepo.UpdateAsync(sub);

                // Al vencer, el negocio pasa al plan gratuito y se aplican sus limites de personal.
                var freePlan = await _planService.GetPlanOrDefault(null);
                await _subRepo.Add(new BusinessSubscription
                {
                    BusinessId = sub.BusinessId,
                    PlanId = freePlan.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(freePlan.DurationDays),
                    Status = Status.Active
                });
                await _staffLimitEnforcer.ApplyStaffLimitAsync(sub.BusinessId, freePlan);

                var email = _emailBuilder.BuildExpiredEmail(businessAdmin.Email, sub.Business.Name);
                await _emailService.SendEmailAsync(email);
            }
        }
    }
}