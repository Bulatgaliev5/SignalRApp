using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.UserFolder
{
    [Table("sessions")]

    public class Session
    {
        [Key]
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public string NameDevice { get; set; }
        public string Model { get; set; }
        public string OS { get; set; }
        public string IP_adress { get; set; }
        public string IP_adress_city { get; set; }
        public string IP_adress_country { get; set; }
        public string IP_adress_CountryFlagURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
