using System.ComponentModel.DataAnnotations;

namespace SignalRApp.Models
{
    public class MessageChatModel
    {
        [Key]
        public int ID { get; set; }
        public int ChatId { get; set; }
        public int CompanionID { get; set; }

        public string CompanionName { get; set; } = null!;
        public string CompanionPhoto { get; set; } = null!;

        public string MessageText { get; set; } = null!;
        public DateTime DateSendMessage { get; set; }
    }
}
