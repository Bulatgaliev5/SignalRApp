using SignalRApp.ChatFolder;
using SignalRApp.UserFolder;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.MessageFolder
{
    [Table("messages")]
    public class Message
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("ChatId")]
        public int ChatId { get; set; }
        [Column("SenderId")]
        public int SenderId { get; set; }

        [Column("MessageText")]
        public string MessageText { get; set; }

        [Column("DateSendMessage")]
        public DateTime DateSendMessage { get; set; }
        [Column("MessageType")]
        public MessageTypeEnum MessageType { get; set; }

        // Навигационное свойство
        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }
        public List<MessageFiles> MessageFiles { get; set; } = new();

    }

    public enum MessageTypeEnum
    {
        Text,
        File
    }
}
