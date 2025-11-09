using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.Models
{
    [Table("messages")]
    public class MessageModel
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("ChatId")]
        public int ChatId { get; set; }
        [Column("SenderId")]
        public int SenderId { get; set; }

        [Column("MessageText")]
        public string MessageText { get; set; } = null!;

        [Column("DateSendMessage")]
        public DateTime DateSendMessage { get; set; }

        // Навигационное свойство
        [ForeignKey("ChatId")]
        public ChatModel Chat { get; set; } = null!;
        [ForeignKey("SenderId")]
        public UserModel Sender { get; set; } = null!;

    }
}
