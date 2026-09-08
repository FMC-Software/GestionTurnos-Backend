using GestionTurnos.Application.Exceptions;

namespace GestionTurnos.Application.Helpers
{
    public static class PlanLimitGuard
    {
        // maxAllowed < 0  => sin límite (no valida)
        // maxAllowed == 0 => no permite crear ninguno
        // maxAllowed > 0  => tope máximo permitido
        public static void EnsureWithinLimit(int currentCount, int maxAllowed, string resourceName)
        {
            if (maxAllowed < 0)
            {
                return;
            }

            if (currentCount >= maxAllowed)
            {
                throw new ConflictException(
                    $"Ha alcanzado el límite de {resourceName} permitido por su plan actual ({maxAllowed}).");
            }
        }
    }
}
