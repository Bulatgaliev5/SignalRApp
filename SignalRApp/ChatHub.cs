using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await this.Clients.All.SendAsync("Receive", user, message);
        }

        // Приватное сообщение по ConnectionId
        public async Task SendPrivateMessage(string targetConnectionId, string user, string message)
        {
            await Clients.Client(targetConnectionId).SendAsync("Receive", user, message);
        }
    }
}
