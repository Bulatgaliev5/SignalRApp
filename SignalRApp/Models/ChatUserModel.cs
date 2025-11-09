namespace SignalRApp.Models
{
    public class ChatUserModel
    {
        public int ChatId { get; set; }
        public string CompanionName { get; set; } = null!;
        public string CompanionPhoto { get; set; } = null!;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }
    }
}
