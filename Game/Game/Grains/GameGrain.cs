using Game.Data.Repositories;
using Game.Models;
using Orleans;

namespace Game.Grains
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task<GameState> AddPlayerToGame(Guid playerId);
        Task FinishGame(Guid playerId);
    }
    public class GameGrain : Grain, IGameGrain
    {
        private List<Guid> playerIds;
        private GameState state;
        private readonly IRoomRepository roomRepository;

        public GameGrain(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }
        public override Task OnActivateAsync()
        {
            playerIds = new List<Guid>();
            state = GameState.AwaitingPlayers;
            return base.OnActivateAsync();
        }

        public async Task<GameState> AddPlayerToGame(Guid playerId)
        {
            playerIds.Add(playerId);
            if (state == GameState.AwaitingPlayers && playerIds.Count == 2)
            {
                state = GameState.InPlay;

                await Task.WhenAll(playerIds.Select(id => GrainFactory.GetGrain<IPlayerGrain>(id).StartGame(this.GetPrimaryKey())));
            }

            return state;
        }

        public async Task FinishGame(Guid playerId)
        {
            if (state != GameState.Finished)
            {
                state = GameState.Finished;
                var loser = GrainFactory.GetGrain<IPlayerGrain>(playerIds.First(x => x != playerId));
                loser.EndGame();
                await loser.SumUp(false);

                var winner = GrainFactory.GetGrain<IPlayerGrain>(playerId);
                await winner.SumUp(true);
                roomRepository.UpdateState(this.GetPrimaryKey(), state);
                await roomRepository.SaveChanges();
            }
        }

    }
}
