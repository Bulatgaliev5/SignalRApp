using SignalRApp.ChatFolder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRApp.MessageFolder
{
    [Table("messagefiles")]
    public class MessageFiles
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("FileURL")]
        public string FileURL { get; set; }
        [Column("FileName")]
        public string FileName { get; set; }
        [Column("TypeFile")]
        public TypeFileEnum TypeFile { get; set; }

        [Column("MessageId")]
        public int MessageId { get; set; }
        // Навигационное свойство
        [ForeignKey("MessageId")]
        public Message Message { get; set; }
    }

    public enum TypeFileEnum
    {
        Image,
        Pdf,
        Video,       
    }
}
