using Microsoft.AspNetCore.SignalR;

namespace SignalRApp.Services
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Берем userId из query-параметра подключения
            // Например: https://server/chatHub?userId=5
            return connection.GetHttpContext()?.Request.Query["userId"];
        }
    }
}
