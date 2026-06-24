namespace Identira.Models
{
    internal class EmailSettings
    {
        public required string ApiKey { get; set; }
        public required string SenderEmail { get; set; }
        public required string SenderName { get; set; }
    }
}
