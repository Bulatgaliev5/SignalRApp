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
            //Database.EnsureCreated(); // Можно оставить, чтобы база создавалась автоматически
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальная пара пользователей в чате
            modelBuilder.Entity<ChatModel>()
                .HasIndex(c => new { c.SenderId, c.RecipientId })
                .IsUnique();

            // Связь Chat -> Sender
            modelBuilder.Entity<ChatModel>()
                .HasOne(c => c.Sender)
                .WithMany(u => u.SentChats)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Связь Chat -> Recipient
            modelBuilder.Entity<ChatModel>()
                .HasOne(c => c.Recipient)
                .WithMany(u => u.ReceivedChats)
                .HasForeignKey(c => c.RecipientId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Связь Chat -> Messages
            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
