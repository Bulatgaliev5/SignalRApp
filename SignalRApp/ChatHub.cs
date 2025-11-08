using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class ChatHub: Hub
    {
        // Словарь пользователей: UserName -> ConnectionId
        private static readonly Dictionary<string, string> Users = new();

        // Подключение
        public override async Task OnConnectedAsync()
        {
            string userName = Context.GetHttpContext().Request.Query["user"];
            if (!string.IsNullOrEmpty(userName))
            {
                Users[userName] = Context.ConnectionId;
                await UpdateUsers();
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = Users.FirstOrDefault(u => u.Value == Context.ConnectionId).Key;
            if (user != null)
            {
                Users.Remove(user);
                await UpdateUsers();
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Отправка общего сообщения
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("Receive", user, message);
        }

        // Отправка приватного сообщения
        public async Task SendPrivateMessage(string targetConnectionId, string user, string message)
        {
            await Clients.Client(targetConnectionId).SendAsync("Receive", user, message);
        }

        // Обновление списка подключенных пользователей
        private Task UpdateUsers()
        {
            var userList = Users.Select(u => new UserData { UserName = u.Key, ConnectionId = u.Value }).ToList();
            return Clients.All.SendAsync("UpdateUsers", userList);
        }
    }
    // Модель пользователя
    public class UserData
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}
