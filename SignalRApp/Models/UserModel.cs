using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SignalRApp.Models
{
    [Table("users")]
    public class UserModel : INotifyPropertyChanged
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


        [Column("Pass")]
        public string Pass { get; set; } = null!;

        // Навигационные свойства
        public ICollection<ChatModel> SentChats { get; set; } = new List<ChatModel>();
        public ICollection<ChatModel> ReceivedChats { get; set; } = new List<ChatModel>();


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
