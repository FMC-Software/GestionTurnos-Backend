using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

public class SubscriptionProcessor
{
    private readonly IBusinessSubscriptionRepository _subRepo;
    private readonly IEmailContentBuilder _emailBuilder;
    private readonly IEmailService _emailService;
    private readonly IStaffRepository _staffRepo;

    public SubscriptionProcessor(IBusinessSubscriptionRepository subRepo,IStaffRepository staffRepo,IEmailContentBuilder emailBuilder,IEmailService emailService)
    {
        _subRepo = subRepo;
        _staffRepo = staffRepo;
        _emailBuilder = emailBuilder;
        _emailService = emailService;
    }

    public async Task ExecuteAsync()
    {
        var activeSubs = await _subRepo.GetActiveSubscriptionsAsync();


        foreach (var sub in activeSubs)
        {
            var businessAdmin = _staffRepo.GetAllGlobal()
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
                var email = _emailBuilder.BuildExpiredEmail(businessAdmin.Email, sub.Business.Name);
                await _emailService.SendEmailAsync(email);
            }
        }
    }
}