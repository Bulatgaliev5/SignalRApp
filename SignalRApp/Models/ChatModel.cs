using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.Models
{
    [Table("chats")]
    public class ChatModel : INotifyPropertyChanged
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("SenderId")]
        public int SenderId { get; set; }

        [Column("RecipientId")]
        public int RecipientId { get; set; }

        // Навигационные свойства
        [ForeignKey("SenderId")]
        public UserModel Sender { get; set; } = null!;

        [ForeignKey("RecipientId")]
        public UserModel Recipient { get; set; } = null!;

        public ICollection<MessageModel> Messages { get; set; } = new List<MessageModel>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
