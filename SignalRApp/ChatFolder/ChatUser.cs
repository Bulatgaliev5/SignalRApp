namespace SignalRApp.ChatFolder
{
    public class ChatUser
    {
        public int ChatId { get; set; }
        public int CompanionID { get; set; }
        public string CompanionName { get; set; }
        public string CompanionPhoto { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }
    }
}
