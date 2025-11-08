using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await this.Clients.All.SendAsync("Receive", user, message);
        }
    }
}
