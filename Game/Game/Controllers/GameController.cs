using Game.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Game.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class GameController : BaseController
    {
        private readonly IGameManager manager;

        public GameController(IGameManager manager)
        {
            this.manager = manager;
        }

        [HttpGet]
        [SwaggerOperation("Get rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var result = await manager.GetRooms();
            return MakeResponse(result);
        }

        [HttpPost]
        [SwaggerOperation("Create room")]
        public async Task<IActionResult> CreateRoom()
        {
            var result = await manager.CreateRoom();
            return MakeResponse(result);
        }

        [HttpPut("{roomId:guid}")]
        [SwaggerOperation("Join room")]
        public async Task<IActionResult> JoinRoom(Guid roomId)
        {
            var result = await manager.JoinRoom(roomId);
            return MakeResponse(result);
        }
    }
}
