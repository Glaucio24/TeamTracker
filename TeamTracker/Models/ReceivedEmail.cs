namespace TeamTracker.Models
{
    public class ReceivedEmail
    {
        public int Id { get; set; }
        public string? SenderEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
