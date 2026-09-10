using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Response
{
    public class BusinessCardResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? UrlLogo { get; set; }
        public TypeBusiness TypeBusiness { get; set; }
        public StatusBusiness Status { get; set; }

        public int BranchCount { get; set; }
        public int StaffCount { get; set; }
        public int ClientCount { get; set; }
    }
}
