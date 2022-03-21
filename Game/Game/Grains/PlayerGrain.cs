using Game.Data.Repositories;
using Game.Hubs;
using Game.Models;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using SignalR.Orleans.Core;

namespace Game.Grains
{
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task<Guid> CreateGame();
        Task<GameState> JoinGame(Guid gameId);
        Task<Guid?> GetGameId();
        Task StartGame(Guid gameId);
        Task SumUp(bool isWin);
        void EndGame();
    }
    public class PlayerGrain : Grain, IPlayerGrain
    {
        private Guid? gameId;
        private int hp;
        private Random random;
        private IDisposable timer;
        private readonly IUserRepository userRepository;
        private IHubContext<PlayerHub> hubContext;

        public PlayerGrain(IUserRepository userRepository, IHubContext<PlayerHub> hubContext)
        {
            this.userRepository = userRepository;
            this.hubContext = hubContext;
        }

        public override Task OnActivateAsync()
        {
            //hubContext = GrainFactory.GetHub<IPlayerHub>();
            random = new Random();
            return base.OnActivateAsync();
        }

        public async Task<Guid> CreateGame()
        {
            var playerId = this.GetPrimaryKey();
            gameId = Guid.NewGuid();
            var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId.Value);
            await gameGrain.AddPlayerToGame(playerId);

            return gameId.Value;
        }

        public async Task<GameState> JoinGame(Guid gameId)
        {
            var playerId = this.GetPrimaryKey();
            var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId);
            return await gameGrain.AddPlayerToGame(playerId);
        }

        public async Task StartGame(Guid gameId)
        {
            hp = 10;
            timer = RegisterTimer(TakeDamage, gameId, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            var playerId = this.GetPrimaryKey();
            var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId);
            var gameState = await gameGrain.AddPlayerToGame(playerId);
        }

        private async Task TakeDamage(object gameId)
        {
            hp -= random.Next(0, 2);
            await hubContext.Clients.All.SendAsync("game", new HealthModel
            {
                Health = hp,
                GameId = (Guid)gameId,
                PlayerId = this.GetPrimaryKey()
            }, CancellationToken.None);
            if (hp <= 0)
            {
                EndGame();
                var gameGrain = GrainFactory.GetGrain<IGameGrain>((Guid)gameId);
                await gameGrain.FinishGame(this.GetPrimaryKey());
            }
        }
        

        public async Task SumUp(bool isWin)
        {
            if(isWin)
                await hubContext.Clients.All.SendAsync("finish", new FinishModel
                {
                    GameId = (Guid)gameId,
                    PlayerId = this.GetPrimaryKey()
                }, CancellationToken.None);
            var user = await userRepository.GetUser(this.GetPrimaryKey());
            user.RoomId = null;
            user.Room = null;
            await userRepository.SaveChanges();
        }

        public Task<Guid?> GetGameId() => Task.FromResult(gameId);

        public void EndGame() => timer?.Dispose();
    }
}
