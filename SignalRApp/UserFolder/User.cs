using SignalRApp.ChatFolder;
using SignalRApp.MessageFolder;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SignalRApp.UserFolder
{
    [Table("users")]
    public class User 
    {
        [Key]
        [Column("IdUser")]
        public int IdUser { get; set; }

        [Column("NameUser")]
        public string NameUser { get; set; } 

        [Column("PhotoUser")]
        public string PhotoUser { get; set; }

        [Column("Login")]
        public string Login { get; set; }
        [Column("ConnectionId")]
        public string? ConnectionId { get; set; } 

        [Column("Pass")]
        public string Pass { get; set; }

        // Навигационные свойства

        // Чаты, где пользователь первый участник
        [InverseProperty("User1")]
        public ICollection<Chat> ChatsAsUser1 { get; set; } = new List<Chat>();
        // Чаты, где пользователь второй участник
        [InverseProperty("User2")]
        public ICollection<Chat> ChatsAsUser2 { get; set; } = new List<Chat>();
        // Сообщения, которые пользователь отправил
        [InverseProperty("Sender")]
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();

    }
}
