using Microsoft.EntityFrameworkCore;
using SignalRApp.Services;

namespace SignalRApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            // Подключаем DbContext с MySQL
            builder.Services.AddDbContext<DataBaze>(options =>
                options.UseMySql(
                    "server=192.168.0.114;user=root;password=yfhenjcfcrt56;database=signalrdb;",
                    new MySqlServerVersion(new Version(8, 0, 43))
                )
            );
            //Настройка Kestrel для Render
            var port = Environment.GetEnvironmentVariable("PORT") ?? "80";

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(int.Parse(port)); // слушаем на порту Render
            });

            var app = builder.Build();

            app.MapHub<ChatHub>("/chat");
            app.Run();
        }
    }
}
