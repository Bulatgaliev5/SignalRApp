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
                    "server=b0tolpm6z0tvjabtprlb-mysql.services.clever-cloud.com;" +
                    "user=utgbhvnerpsf6soi;" +
                    "password=tsKufIRfTz6kcSO8GfWG;" +
                    "database=b0tolpm6z0tvjabtprlb;",
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
