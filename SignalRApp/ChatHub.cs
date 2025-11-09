using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRApp.Models;
using SignalRApp.Services;

namespace SignalRApp
{

    public class ChatHub: Hub
    {
        // Демо база пользователей (логин → пароль)
        //private static readonly Dictionary<string, string> demoUsers = new()
        //{
        //    { "admin", "12345" },
        //    { "user", "qwerty" },
        //    { "test", "test" }
        //};

        private readonly DataBaze _context;
        public ChatHub(DataBaze context)
        {
            _context = context;
        }
        public async Task<bool> AuthorizeUser(string login, string password)
        {
            // Пример: поиск пользователя в БД
            var user = _context.User.FirstOrDefault(u => u.Login == login && u.Pass == password);
           
           // var message = _context.Message.FirstOrDefault();

           // var chat = _context.ChartUser.FirstOrDefault();

            if (user != null)
            {
                // Можно запомнить ConnectionId
                // Например: user.ConnectionId = Context.ConnectionId;
                // await _context.SaveChangesAsyncБ();
                return true;
            }

            return false;
        }

        public async Task<List<ChartUser>> ChatUser(string login)
        {
            // Загружаем пользователя со всеми чатами и сообщениями
            var user = await _context.User
                .Include(u => u.ChartUsers)
                    .ThenInclude(c => c.Message)
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
            {
                return new List<ChartUser>(); // пользователь не найден — возвращаем пустой список
            }

            return user.ChartUsers.ToList();
        }
        

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync("Receive", userName, message);
        }


    }
}
