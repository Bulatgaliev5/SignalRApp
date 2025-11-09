using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.Models
{
    [Table("charts")]
    public class ChartUser : INotifyPropertyChanged
    {
        private int id;
        [Key]
        public int Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("UserId")]        // 👈 имя колонки в БД
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserModel User { get; set; }



        [Column("MessageId")]        // 👈 имя колонки в БД
        public int MessageId { get; set; }
        [ForeignKey(nameof(MessageId))]

        public MessageModel Message { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
