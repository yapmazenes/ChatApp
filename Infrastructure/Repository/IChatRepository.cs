using ChatApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repository
{
    public interface IChatRepository
    {
        Chat GetChat(int id);
        IEnumerable<Chat> GetChats(string userId);
        IEnumerable<Chat> GetPrivateChats(string userId);
        Task CreateRoom(string name, string userId);
        Task<int> CreatePrivateRoom(string rootId, string targetId);
        Task JoinRoom(int chatId, string userId);
        Task<Message> CreateMessage(int chatId, string message, string userId);
    }
}
