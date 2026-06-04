namespace GestionTurnos.Application.Request
{
    public class ServiceRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public decimal Price { get; set; }
    }
}
