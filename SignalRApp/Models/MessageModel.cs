using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.Models
{
    [Table("messages")]
    public class MessageModel : INotifyPropertyChanged
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("ChatId")]
        public int ChatId { get; set; }

        [Column("MessageText")]
        public string MessageText { get; set; } = null!;

        [Column("DateSendMessage")]
        public DateTime DateSendMessage { get; set; }

        // Навигационное свойство
        [ForeignKey("ChatId")]
        public ChatModel Chat { get; set; } = null!;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
