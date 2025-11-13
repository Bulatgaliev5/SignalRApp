using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SignalRApp.Models
{
    [Table("users")]
    public class UserModel 
    {
        [Key]
        [Column("IdUser")]
        public int IdUser { get; set; }

        [Column("NameUser")]
        public string NameUser { get; set; } = null!;

        [Column("PhotoUser")]
        public string PhotoUser { get; set; } = null!;

        [Column("Login")]
        public string Login { get; set; } = null!;
        [Column("ConnectionId")]
        public string? ConnectionId { get; set; } // <-- добавляем это поле

        [Column("Pass")]
        public string Pass { get; set; } = null!;

        // Навигационные свойства

        // Чаты, где пользователь первый участник
        [InverseProperty("User1")]
        public ICollection<ChatModel> ChatsAsUser1 { get; set; } = new List<ChatModel>();
        // Чаты, где пользователь второй участник
        [InverseProperty("User2")]
        public ICollection<ChatModel> ChatsAsUser2 { get; set; } = new List<ChatModel>();
        // Сообщения, которые пользователь отправил
        [InverseProperty("Sender")]
        public ICollection<MessageModel> SentMessages { get; set; } = new List<MessageModel>();

    }
}
