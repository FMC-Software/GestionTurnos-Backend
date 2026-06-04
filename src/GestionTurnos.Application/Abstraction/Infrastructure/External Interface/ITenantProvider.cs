namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface ITenantProvider
    {
        Guid? GetBusinessId();
        Guid? GetBranchId();
        Guid? GetUserId();
        string? GetUserRole();
    }
}