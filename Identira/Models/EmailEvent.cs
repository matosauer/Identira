namespace Identira.Models
{
    public class EmailEvent
    {
        public required string MessageId { get; set; }
        public string? Email { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }
    }
}
