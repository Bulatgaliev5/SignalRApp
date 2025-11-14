using Microsoft.EntityFrameworkCore;
using SignalRApp.ChatFolder;
using SignalRApp.MessageFolder;
using SignalRApp.UserFolder;
using System;

namespace SignalRApp.Services
{
    public class DataBaze : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DataBaze(DbContextOptions<DataBaze> options)
            : base(options)
        {

        }


    }
}
