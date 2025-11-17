
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.EntityFrameworkCore;
using SignalRApp.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace SignalRApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            // Инициализация Firebase
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("signalr-3e0a0-firebase-adminsdk-fbsvc-bf0faf1ab0.json")
            });
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
