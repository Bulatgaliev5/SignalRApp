using SignalRApp.MessageFolder;
using SignalRApp.UserFolder;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.ChatFolder
{
    [Table("chats")]
    public class Chat
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("User1Id")]
        public int User1Id { get; set; }

        [Column("User2Id")]
        public int User2Id { get; set; }


        // Навигационные свойства
        [ForeignKey("User1Id")]
        public User User1 { get; set; }
        [ForeignKey("User2Id")]
        public User User2 { get; set; }
        [InverseProperty("Chat")]
        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
