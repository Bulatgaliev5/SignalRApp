using Microsoft.EntityFrameworkCore;
using SignalRApp.Models;

namespace SignalRApp.Services
{
    public class DataBaze : DbContext
    {
        public DbSet<UserModel> User { get; set; }
        public DbSet<MessageModel> Message { get; set; }
        public DbSet<ChartUser> ChartUser { get; set; }

        // ✅ Конструктор для DI
        public DataBaze(DbContextOptions<DataBaze> options)
            : base(options)
        {
            //Database.EnsureCreated(); // Можно оставить, чтобы база создавалась автоматически
        }
    }
}
