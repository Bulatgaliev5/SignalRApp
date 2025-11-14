using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRApp.ChatFolder;
using SignalRApp.MessageFolder;
using SignalRApp.Services;
using SignalRApp.UserFolder;
using System.Reflection;

namespace SignalRApp
{

    public class ChatHub: Hub
    {
        public ChatHub(DataBaze context)
        {
            this.context = context;
        }
        #region Поля и свойства
        private readonly DataBaze context;
        #endregion
        #region Методы
        public async Task<User> AuthorizeUser(string login, string password)
        {
            // Пример: поиск пользователя в БД
            var user = context.Users.FirstOrDefault(u => u.Login == login && u.Pass == password);

            if (user != null)
            {
                return user;
            }

            return null;
        }
        public async Task SaveConnectionId(int userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.IdUser == userId);
            if (user != null)
            {
                user.ConnectionId = Context.ConnectionId;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
        }
        public async Task<List<ChatUser>> GetUserChats(string login)
        {
            // Находим текущего пользователя
            var currentUser = await context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (currentUser == null)
                return new List<ChatUser>();

            int currentUserId = currentUser.IdUser;

            // Получаем все чаты, где участвует пользователь
            var chats = await context.Chats
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

                return new ChatUser
                {
                    ChatId = c.Id,
                    CompanionName = companion.NameUser,
                    CompanionPhoto = companion.PhotoUser,
                    CompanionID = companion.IdUser,
                    LastMessage = lastMessage?.MessageText,
                    LastMessageDate = lastMessage?.DateSendMessage
                };
            })
            // Сортировка по дате последнего сообщения
            .OrderByDescending(c => c.LastMessageDate)
            .ToList();

            return chatList;
        }


        public async Task<List<MessageChat>> GetSelectedChatUser(int ChatId)
        {
            var messages = await context.Messages
                .Include(m => m.Sender) 
                .Include(m => m.Chat)
                .Where(m => m.ChatId == ChatId)
                .OrderBy(m => m.DateSendMessage)
                .Select(m => new MessageChat
                {
                    ID = m.ID,
                    ChatId = m.ChatId,
                    CompanionID = m.Sender.IdUser,
                    CompanionName = m.Sender.NameUser,
                    CompanionPhoto = m.Sender.PhotoUser,
                    MessageText = m.MessageText,
                    DateSendMessage = m.DateSendMessage
                })
                .ToListAsync();

            return messages;
        }

        public async Task<bool> SentMessageUser(int ChatId, int CompanionID, 
            int IdUser, string Message, DateTime dateTime, string ConnectionId)
        {

            try
            {
                var msg = new Message
                {
                    ChatId = ChatId,
                    SenderId = IdUser,
                    MessageText = Message,
                    DateSendMessage = DateTime.Now
                };

                // ✅ 1. Сохраняем сообщение в БД
                context.Messages.Add(msg);
                await context.SaveChangesAsync();

                // ✅ 2. Отправляем сообщение обоим участникам чата
                // Чтобы работало, пользователи должны быть "зарегистрированы" по ConnectionId
                // Создаём типизированную модель для отправки клиенту
                var sender = await context.Users.FirstOrDefaultAsync(u => u.IdUser == IdUser);
                var messageDto = new MessageChat
                {
                    ID = msg.ID,
                    ChatId = msg.ChatId,
                    CompanionID = msg.SenderId,
                    CompanionName = sender?.NameUser ?? "",
                    CompanionPhoto = sender?.PhotoUser ?? "",
                    MessageText = msg.MessageText,
                    DateSendMessage = msg.DateSendMessage
                };

                // Находим получателя
                var receiver = await context.Users.FirstOrDefaultAsync(u => u.IdUser == CompanionID);

                // Отправляем получателю
                if (!string.IsNullOrEmpty(receiver?.ConnectionId))
                {
                    await Clients.Client(receiver.ConnectionId).SendAsync("ReceiveMessage", messageDto);
                }

                // Отправляем самому отправителю (для отображения своего сообщения)
                if (!string.IsNullOrEmpty(sender?.ConnectionId))
                {
                    await Clients.Client(sender.ConnectionId).SendAsync("ReceiveMessage", messageDto);
                }

                return true;
            

               
            }
            catch (Exception ex)
            {
                // логируем ошибку
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync("Receive", userName, message);
        }
        #endregion

    }
}
