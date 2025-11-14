using System.ComponentModel.DataAnnotations;

namespace SignalRApp.MessageFolder
{
    public class MessageChat
    {
        [Key]
        public int ID { get; set; }
        public int ChatId { get; set; }
        public int CompanionID { get; set; }

        public string CompanionName { get; set; } 
        public string CompanionPhoto { get; set; }

        public string MessageText { get; set; }
        public DateTime DateSendMessage { get; set; }
    }
}
