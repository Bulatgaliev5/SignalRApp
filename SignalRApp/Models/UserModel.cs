using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SignalRApp.Models
{
    [Table("users")]
    public class UserModel : INotifyPropertyChanged
    {
        private int idUser;
        private string nameUser;
        private string photoUser;

        public string Login { get; set; }
        public string Pass { get; set; }
        [Key]
        public int IdUser
        {
            get => idUser;
            set
            {
                if (idUser != value)
                {
                    idUser = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NameUser
        {
            get => nameUser;
            set
            {
                if (nameUser != value)
                {
                    nameUser = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PhotoUser
        {
            get => photoUser;
            set
            {
                if (photoUser != value)
                {
                    photoUser = value;
                    OnPropertyChanged();
                }
            }
        }

        // ✅ Добавляем связь с ChartUser
        [JsonIgnore]
        public ICollection<ChartUser> ChartUsers { get; set; } = new List<ChartUser>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
