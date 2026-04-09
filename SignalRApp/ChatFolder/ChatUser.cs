using SignalRApp.MessageFolder;
using static SignalRApp.UserFolder.Enums.UserEnums;

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
        public StatusUser? CompanionStatus { get; set; }
        public string? CompanionPublicKey { get; set; }

        public MessageTypeEnum MessageType { get; set; }
        public List<FileDto> MessageFiles { get; set; } = new();
    }
}
