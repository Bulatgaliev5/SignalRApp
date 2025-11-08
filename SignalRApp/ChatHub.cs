using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    //[Authorize]
    public class ChatHub: Hub
    {
        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync("Receive", userName, message);
        }


    }
}
