namespace TeamTracker.Models
{
    public class SentEmail
    {
        public int Id { get; set; }
        public string? RecipientEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime SentDate { get; set; }
    }
}
