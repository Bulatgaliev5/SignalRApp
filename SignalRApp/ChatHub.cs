using FirebaseAdmin.Auth;
using IPinfo;
using IPinfo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SignalRApp.ChatFolder;
using SignalRApp.MessageFolder;
using SignalRApp.Services;
using SignalRApp.UserFolder;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static SignalRApp.UserFolder.Enums.UserEnums;

namespace SignalRApp
{

    public class ChatHub : Hub
    {
        public ChatHub(DataBaze context)
        {
            this.context = context;

        }
        #region Поля и свойства
        private readonly DataBaze context;
        private User User;
        #endregion
        #region Методы

        public async Task<User> AuthorizeFirebase(string idToken)
        {
            try
            {
                // Проверяем токен Firebase
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);

                string firebaseUid = decodedToken.Uid;
                string email = decodedToken.Claims["email"].ToString();

                // Ищем пользователя в БД
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
                User = user;
                // Сохраняем ConnectionId
                user.ConnectionId = Context.ConnectionId;
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Auth error: " + ex.Message);
                return null;
            }
        }
        public async Task<User> RegistrationFirebase(string idToken, User user1)
        {
            try
            {
                // Проверяем токен Firebase
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(idToken);

                string firebaseUid = decodedToken.Uid;
                string email = decodedToken.Claims["email"].ToString();

                // Ищем пользователя в БД
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    return null;
                }

                // Если нет  регистрируем автоматически
                if (user == null)
                {
                    user = new User
                    {
                        Email = email,
                        Pass = user1.Pass,
                        PhotoUser = user1.PhotoUser,
                        Nickname = email.Split('@')[0],
                        NameUser = user1.NameUser,
                        ConnectionId = Context.ConnectionId
                    };

                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                // Сохраняем ConnectionId
                user.ConnectionId = Context.ConnectionId;
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Auth error: " + ex.Message);
                return null;
            }
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
        public async Task<List<ChatUser>> GetUserChats(string email)
        {
            var currentUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

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
                var companion = c.User1Id == currentUserId ? c.User2 : c.User1;

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
                    LastMessageDate = lastMessage?.DateSendMessage,
                    CompanionStatus = (StatusUser)companion.Status
                };
            })
            // Сортировка по дате последнего сообщения
            .OrderByDescending(c => c.LastMessageDate)
            .ToList();

            return chatList;
        }

        public async Task<ChatUser> GetUserChat(string email, int companionId)
        {
            var currentUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null)
                return null;

            int currentUserId = currentUser.IdUser;

            // Ищем чат только между двух пользователей
            var chat = await context.Chats
                .Include(c => c.Messages)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == currentUserId && c.User2Id == companionId) ||
                    (c.User2Id == currentUserId && c.User1Id == companionId)
                );

            // Если нет — создаём
            if (chat == null)
            {
                chat = new Chat
                {
                    User1Id = currentUserId,
                    User2Id = companionId,
                };

                context.Chats.Add(chat);
                await context.SaveChangesAsync();

                // Загружаем пользователей
                chat = await context.Chats
                    .Include(c => c.User1)
                    .Include(c => c.User2)
                    .FirstOrDefaultAsync(c => c.Id == chat.Id);
            }

            // Определяем собеседника
            var companion = chat.User1Id == currentUserId ? chat.User2 : chat.User1;
            var lastMessage = chat.Messages
                .OrderByDescending(m => m.DateSendMessage)
                .FirstOrDefault();
            if (chat.Messages.Count == 0)
            {
                lastMessage = new Message
                {
                    ChatId = chat.Id,
                    SenderId = currentUserId,
                    MessageText = "Привет!",
                    DateSendMessage = DateTime.Now,
                };

                context.Messages.Add(lastMessage);
                await context.SaveChangesAsync();
            }
            //// Последнее сообщение
            //var lastMessage = chat.Messages
            //    .OrderByDescending(m => m.DateSendMessage)
            //    .FirstOrDefault();
            // Отправляем получателю

            var chatuser = new ChatUser
            {
                ChatId = chat.Id,
                CompanionName = companion.NameUser,
                CompanionPhoto = companion.PhotoUser,
                CompanionID = companion.IdUser,
                LastMessage = lastMessage?.MessageText,
                LastMessageDate = lastMessage?.DateSendMessage
            };
            //if (!string.IsNullOrEmpty(receiver?.ConnectionId))
            //{
            //    await Clients.Client(receiver.ConnectionId).SendAsync("ReceiveMessage", chatuser);
            //}

            // Отправляем самому отправителю (для отображения своего сообщения)
            //if (!string.IsNullOrEmpty(currentUser?.ConnectionId))
            //{
            //    await Clients.Client(currentUser.ConnectionId).SendAsync("ReceiveUserChat", chatuser);
            //}
            if (!string.IsNullOrEmpty(currentUser.ConnectionId))
            {
                await Clients.Client(currentUser.ConnectionId).SendAsync("ReceiveUserChat", chatuser);
            }

            if (!string.IsNullOrEmpty(companion.ConnectionId))
            {
                await Clients.Client(companion.ConnectionId).SendAsync("ReceiveUserChat", chatuser);
            }
            return chatuser;
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

        public async Task<List<User>> SearchUserServer(string nickname, User user)
        {
            //var users = await context.Users
            //    .Where(u => u.Nickname == nickname)
            //    .ToListAsync();
            var users = await context.Users
                .Where(u => EF.Functions.Like(u.Nickname!, $"%{nickname}%") && u.Nickname != user.Nickname).ToListAsync();

            return users;
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

                context.Messages.Add(msg);
                await context.SaveChangesAsync();

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

                var receiver = await context.Users.FirstOrDefaultAsync(u => u.IdUser == CompanionID);

                if (!string.IsNullOrEmpty(receiver?.ConnectionId))
                {
                    await Clients.Client(receiver.ConnectionId).SendAsync("ReceiveMessage", messageDto);
                }

                if (!string.IsNullOrEmpty(sender?.ConnectionId))
                {
                    await Clients.Client(sender.ConnectionId).SendAsync("ReceiveMessage", messageDto);
                }

                return true;



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        //Метод создания сессии, если нету
        public async Task<Session> CreateSession(User user, string os, string model, string nameDevice)
        {
            var infoIP = GetInfo_IP_adress().Result;
            var newSession = new Session
            {
                // SessionId = Guid.NewGuid().ToString(),
                UserId = user.IdUser,
                RefreshToken = Guid.NewGuid().ToString(),
                OS = os,
                Model = model,
                NameDevice = nameDevice,
                IP_adress = infoIP[0].ToString(),
                IP_adress_country = infoIP[1].ToString(),
                IP_adress_city = infoIP[2].ToString(),
                IP_adress_CountryFlagURL = infoIP[3].ToString(),
                CreatedAt = DateTime.UtcNow,
                // 7 дней действует сессия
                ExpiresAt = DateTime.UtcNow.AddDays(7) //Не влияет ни на что 
            };

            context.Sessions.Add(newSession);
            await context.SaveChangesAsync();

            return newSession;
        }
        //Метод повторной авторизации, когда есть сессия

        public async Task<User> AuthorizeByRefresh(string refreshToken)
        {
            var session = await context.Sessions
                .Include(s => s.User)
                //.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.ExpiresAt > DateTime.UtcNow);
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

            if (session == null)
                return null;

            session.User.ConnectionId = Context.ConnectionId;

            await context.SaveChangesAsync();
            return session.User;
        }

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.SendAsync("Receive", userName, message);
        }

        public async Task<List<Session>> GetLoadSessions_DB(string email)
        {
            var currentUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null)
                return new List<Session>();

            int currentUserId = currentUser.IdUser;
            //731e5fd7-b90c-4599-b8b1-a91796291243
            var sessions = await context.Sessions
                .Include(u => u.User)
                .Where(u => u.UserId == currentUserId)
                .ToListAsync();

            return sessions;
        }
        public async Task<List<string>> GetInfo_IP_adress()
        {
            List<string> list = new List<string>();
            string token = "58c696a42265bd";

            IPinfoClient client = new IPinfoClient.Builder()
                .AccessToken(token)
                .Build();
            IPResponse ipResponse = await client.IPApi.GetDetailsAsync();

            list.Add(ipResponse.IP);
            list.Add(ipResponse.CountryName);
            list.Add(ipResponse.City);
            list.Add(ipResponse.CountryFlagURL);

            //list.Add(ipResponse.CountryFlagURL);

            return list;
        }

        //public override async Task OnConnectedAsync()
        //{
        //    var connectionId = Context.ConnectionId;

        //    var receiver = await context.Users.FirstOrDefaultAsync(u => u.ConnectionId == User.ConnectionId);

        //    if (receiver != null)
        //         await Clients.Client(receiver.ConnectionId).SendAsync("UserStatus", StatusUser.Online); //в сети
            


        //    //await Clients.All.SendAsync("UserStatusConnected", connectionId);

        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var connectionId = Context.ConnectionId;

        //    var receiver = await context.Users.FirstOrDefaultAsync(u => u.ConnectionId == User.ConnectionId);


        //    if (receiver != null)
        //        await Clients.Client(receiver.ConnectionId).SendAsync("UserStatus", StatusUser.Offline); //не в сети

        //    //await Clients.All.SendAsync("UserDisconnected", connectionId);

        //    await base.OnDisconnectedAsync(exception);
        //}


        //public override async Task OnConnectedAsync()
        //{
        //    var connectionId = Context.ConnectionId;

        //    //if (Context.ConnectionAborted.IsCancellationRequested)
        //    {
        //        var receivers = await context.Chats
        //            .Include(s => s.User2.ConnectionId == connectionId)
        //            .ToListAsync();
        //        //var receiver = await context.Users.FirstOrDefaultAsync(u => u.IdUser == CompanionID);

        //        foreach (var receiver in receivers)
        //        {
        //            if (!string.IsNullOrEmpty(receiver?.User2.ConnectionId))
        //            {
        //                await Clients.Client(receiver.User2.ConnectionId).SendAsync("UserStatus", StatusUser.Online); //в сети
        //            }
        //        }
        //    }





        //    //await Clients.All.SendAsync("UserStatusConnected", connectionId);

        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var connectionId = Context.ConnectionId;

        //    //if (!Context.ConnectionAborted.IsCancellationRequested)
        //    {
        //        var receivers = await context.Chats
        //        .Include(s => s.User2.ConnectionId == connectionId)
        //        .ToListAsync();
        //        //var receiver = await context.Users.FirstOrDefaultAsync(u => u.IdUser == CompanionID);

        //        foreach (var receiver in receivers)
        //        {
        //            if (!string.IsNullOrEmpty(receiver?.User2.ConnectionId))
        //            {
        //                await Clients.Client(receiver.User2.ConnectionId).SendAsync("UserStatus", StatusUser.Offline); //не в сети
        //            }
        //        }
        //        //await Clients.All.SendAsync("UserStatusConnected", connectionId);
        //    }
        //    await base.OnDisconnectedAsync(exception);
        //}
        #endregion


    }
}
