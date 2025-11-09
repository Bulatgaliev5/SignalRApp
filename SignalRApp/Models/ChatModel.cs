using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.Models
{
    [Table("chats")]
    public class ChatModel
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
        public UserModel User1 { get; set; } = null!;
        [ForeignKey("User2Id")]
        public UserModel User2 { get; set; } = null!;
        [InverseProperty("Chat")]
        public ICollection<MessageModel> Messages { get; set; } = new List<MessageModel>();

    }
}
