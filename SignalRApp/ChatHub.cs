using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRApp.Models;
using SignalRApp.Services;

namespace SignalRApp
{

    public class ChatHub: Hub
    {
        private readonly DataBaze _context;
        public ChatHub(DataBaze context)
        {
            _context = context;
        }
        public async Task<bool> AuthorizeUser(string login, string password)
        {
            // Пример: поиск пользователя в БД
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Pass == password);

            if (user != null)
            {
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

            int currentUserId = currentUser.IdUser;

            // Получаем все чаты, где участвует пользователь
            var chats = await _context.Chats
                .Include(c => c.Messages)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Where(c => c.User1Id == currentUserId || c.User2Id == currentUserId)
                .ToListAsync();

            var chatList = chats.Select(c =>
            {
                // Определяем собеседника
                var companion = c.User1Id == currentUserId ? c.User2 : c.User1;

                // Берем последнее сообщение в чате
                var lastMessage = c.Messages
                    .OrderByDescending(m => m.DateSendMessage)
                    .FirstOrDefault();

                return new ChatUserModel
                {
                    ChatId = c.Id,
                    CompanionName = companion.NameUser,
                    CompanionPhoto = companion.PhotoUser,
                    LastMessage = lastMessage?.MessageText,
                    LastMessageDate = lastMessage?.DateSendMessage
                };
            })
            // Сортировка по дате последнего сообщения
            .OrderByDescending(c => c.LastMessageDate)
            .ToList();

            return chatList;
        }
        

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync("Receive", userName, message);
        }


    }
}
