using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Presentation.Authorization
{


    public static class Policies
    {
        public const string Admin = nameof(Rol.Admin);
        public const string Recepcionista = nameof(Rol.Recepcionista);
        public const string Profesional = nameof(Rol.Profesional);
    }
}
