namespace GestionTurnos.Domain.Entities
{
    // Pregunta frecuente de la seccion "Ayuda" del dashboard. Lista dinamica
    // (el SysAdmin puede agregar/editar/eliminar), no un numero fijo.
    public class Faq : BaseEntity
    {
        public int Order { get; set; }
        public string QuestionEs { get; set; } = string.Empty;
        public string QuestionEn { get; set; } = string.Empty;
        public string AnswerEs { get; set; } = string.Empty;
        public string AnswerEn { get; set; } = string.Empty;
    }
}
