namespace GestionTurnos.Application.Response
{
    public class SysAdminProfileResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LinkPhoto { get; set; } = string.Empty;
    }
}
