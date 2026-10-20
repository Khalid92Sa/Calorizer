namespace Calorizer.Business.DTOs
{
    public class CalculationNoteDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
