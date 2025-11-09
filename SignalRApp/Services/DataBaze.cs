using Microsoft.EntityFrameworkCore;
using SignalRApp.Models;
using System;

namespace SignalRApp.Services
{
    public class DataBaze : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<ChatModel> Chats { get; set; } = null!;
        public DbSet<MessageModel> Messages { get; set; } = null!;

        // ✅ Конструктор для DI
        public DataBaze(DbContextOptions<DataBaze> options)
            : base(options)
        {

        }


    }
}
