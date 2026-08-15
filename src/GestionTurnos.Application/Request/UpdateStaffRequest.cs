namespace GestionTurnos.Application.Request
{
    public class UpdateStaffRequest
    {
        public string StaffName { get; set; } = string.Empty;

        public string StaffEmail { get; set; } = string.Empty;

        public string StaffPhone { get; set; } = string.Empty;

        public string StaffLinkPhoto { get; set; } = string.Empty;
    }
}