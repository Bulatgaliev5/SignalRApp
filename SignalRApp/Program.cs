namespace SignalRApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            // Настройка Kestrel для Render
            //var port = Environment.GetEnvironmentVariable("PORT") ?? "80";

            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    options.ListenAnyIP(int.Parse(port)); // слушаем на порту Render
            //});

            var app = builder.Build();

            app.MapHub<ChatHub>("/chat");
            app.Run();
        }
    }
}
