using Game.Data.Repositories;
using Game.Grains;
using Game.Infrastructure;
using Game.Models;
using Orleans;
using System.Security.Claims;

namespace Game.Managers
{
    public interface IGameManager
    {
        Task<Response<Guid>> CreateRoom();
        Task<Response<List<Guid>>> GetRooms();
        Task<Response> JoinRoom(Guid roomId);
    }
    public class GameManager : IGameManager
    {
        private readonly IGrainFactory grainFactory;
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly ClaimsPrincipal user;

        public GameManager(IRoomRepository roomRepository, IUserRepository userRepository, IGrainFactory grainFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.grainFactory = grainFactory;
            user = httpContextAccessor.HttpContext.User;
        }

        public async Task<Response<Guid>> CreateRoom()
        {
            try
            {
                var playerId = user.GetPlayerId();
                var userModel = await userRepository.GetUser(playerId);
                if(userModel == null)
                {
                    return Response<Guid>.Unauthorized();
                }

                if(userModel.RoomId != null)
                {
                    return Response<Guid>.BadRequest($"{userModel.Username} in room {userModel.RoomId}");
                }
                var playerGrain = grainFactory.GetGrain<IPlayerGrain>(playerId);
                var gameId = await playerGrain.CreateGame();
                var room = new RoomModel
                {
                    RoomId = gameId,
                    State = GameState.AwaitingPlayers,
                };
                room.Users.Add(userModel);
                await roomRepository.CreateRoom(room);
                await roomRepository.SaveChanges();

                return Response<Guid>.Ok(gameId);
            }
            catch (Exception ex)
            {
                return Response<Guid>.InternalServerError(ex.Message);
            }
        }

        public async Task<Response<List<Guid>>> GetRooms()
        {
            try
            {
                var rooms = await roomRepository.GetFreeRooms();
                return Response<List<Guid>>.Ok(rooms.Select(x => x.RoomId).ToList());
            }
            catch (Exception ex)
            {
                return Response<List<Guid>>.InternalServerError(ex.Message);
            }
        }

        public async Task<Response> JoinRoom(Guid roomId)
        {
            try
            {
                var playerId = user.GetPlayerId();
                var room = await roomRepository.GetRoom(roomId);
                if(room == null)
                {
                    return Response.NotFound();
                }

                if(room.State != GameState.AwaitingPlayers)
                {
                    return Response.BadRequest($"Room {roomId} state: {room.State}");
                }

                var playerGrain = grainFactory.GetGrain<IPlayerGrain>(playerId);
                var state = await playerGrain.JoinGame(roomId);
                roomRepository.UpdateState(roomId, state);
                await roomRepository.SaveChanges();

                return Response.Ok();
            }
            catch (Exception ex)
            {
                return Response.InternalServerError(ex.Message);
            }
        }
    }
}
