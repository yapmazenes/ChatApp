using ChatApp.Database;
using ChatApp.Hubs;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IChatRepository _chatRepository;

        public HomeController(IChatRepository chatRepository) => _chatRepository = chatRepository;

        public IActionResult Index()
        {
            var chats = _chatRepository.GetChats(GetUserId());

            return View(chats);
        }

        public IActionResult Find([FromServices] AppDbContext _ctx)
        {
            var users = _ctx.Users.Where(x => x.Id != User.GetUserId()).ToList();

            return View(users);
        }

        public IActionResult Private()
        {
            var chats = _chatRepository.GetPrivateChats(GetUserId());

            return View(chats);
        }

        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var id = await _chatRepository.CreatePrivateRoom(GetUserId(), userId);

            return RedirectToAction("Chat", new { id });
        }

        [HttpGet("{id}")]
        public IActionResult Chat(int id = 1) => View(_chatRepository.GetChat(id));

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            await _chatRepository.CreateRoom(name, GetUserId());

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinRoom(int id)
        {
            await _chatRepository.JoinRoom(id, GetUserId());

            return RedirectToAction("Chat", "Home", new { id });
        }

        public async Task<IActionResult> SendMessage(
            int roomId,
            string message,
            [FromServices] IHubContext<ChatHub> _hubContext)
        {
            var Message = await _chatRepository.CreateMessage(roomId, message, User.Identity.Name);

            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", new
            {
                Text = Message.Text,
                Name = Message.Name,
                TimeStamp = Message.TimeStamp.ToString("dd/MM/yyyy hh:mm:ss tt")
            });

            return Ok();
        }
    }
}