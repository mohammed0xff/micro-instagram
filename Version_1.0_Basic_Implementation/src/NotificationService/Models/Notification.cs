namespace NotificationService.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ReadAt { get; set; }
    }
}
