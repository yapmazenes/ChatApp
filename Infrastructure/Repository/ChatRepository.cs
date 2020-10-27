﻿using ChatApp.Database;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _ctx;

        public ChatRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<Message> CreateMessage(int chatId, string message, string userId)
        {
            var Message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = userId,
                TimeStamp = DateTime.Now
            };

            _ctx.Messages.Add(Message);

            await _ctx.SaveChangesAsync();

            return Message;

        }

        public async Task<int> CreatePrivateRoom(string rootId, string targetId)
        {
            var chat = new Chat
            {
                Type = ChatType.Private,
            };

            chat.Users.Add(new ChatUser
            {
                UserId = targetId
            });

            chat.Users.Add(new ChatUser
            {
                UserId = rootId
            });

            _ctx.Chats.Add(chat);

            await _ctx.SaveChangesAsync();

            return chat.Id;
        }

        public async Task CreateRoom(string name, string userId)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room
            };

            chat.Users.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Admin,

            });

            _ctx.Chats.Add(chat);

            await _ctx.SaveChangesAsync();
        }

        public Chat GetChat(int id) =>
            _ctx.Chats
                .Include(x => x.Messages)
                .FirstOrDefault(x => x.Id == id);

        public IEnumerable<Chat> GetChats(string userId) =>

            _ctx.Chats.Include(x => x.Users)
                .Where(y => !y.Users
                .Any(x => x.UserId == userId))
                .ToList();

        public IEnumerable<Chat> GetPrivateChats(string userId) =>

            _ctx.Chats.Include(x => x.Users).ThenInclude(x => x.User)
               .Where(x => x.Type == ChatType.Private && x.Users
               .Any(x => x.UserId == userId))
               .ToList();

        public async Task JoinRoom(int chatId, string userId)
        {
            var chatUser = new ChatUser
            {
                ChatId = chatId,
                UserId = userId,
                Role = UserRole.Member,

            };

            _ctx.ChatUsers.Add(chatUser);

            await _ctx.SaveChangesAsync();

        }
    }
}
