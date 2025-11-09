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
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Pass == password);
           
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

        public async Task<List<ChatUserModel>> GetUserChats(string login)
        {
            // Находим текущего пользователя
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (currentUser == null)
                return new List<ChatUserModel>();

            // Получаем все чаты, где он — либо отправитель, либо получатель
            var chats = await _context.Chats
                .Include(c => c.Sender)
                .Include(c => c.Recipient)
                .Include(c => c.Messages)
                .Where(c => c.SenderId == currentUser.IdUser || c.RecipientId == currentUser.IdUser)
                .ToListAsync();

            // Преобразуем данные в удобную модель
            var result = chats.Select(chat =>
            {
                // Определяем, кто собеседник (не текущий пользователь)
                var companion = chat.SenderId == currentUser.IdUser
                    ? chat.Recipient
                    : chat.Sender;

                // Получаем последнее сообщение (если есть)
                var lastMessage = chat.Messages
                    .OrderByDescending(m => m.DateSendMessage)
                    .FirstOrDefault();

                return new ChatUserModel
                {
                    ChatId = chat.Id,
                    CompanionName = companion.NameUser,
                    CompanionPhoto = companion.PhotoUser,
                    LastMessage = lastMessage?.MessageText,
                    LastMessageDate = lastMessage?.DateSendMessage
                };
            }).ToList();

            return result;
        }
        

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync("Receive", userName, message);
        }


    }
}
