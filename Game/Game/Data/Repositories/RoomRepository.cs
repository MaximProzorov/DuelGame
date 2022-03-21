using Game.Models;
using Microsoft.EntityFrameworkCore;

namespace Game.Data.Repositories
{
    public interface IRoomRepository
    {
        Task CreateRoom(RoomModel room);
        Task<List<RoomModel>> GetFreeRooms();
        Task<RoomModel> GetRoom(Guid roomId);
        void UpdateState(Guid roomId, GameState state);
        Task SaveChanges();
    }
    public class RoomRepository : IRoomRepository
    {
        private readonly DbContext context;
        private DbSet<RoomModel> dbSet => context.Set<RoomModel>();

        public RoomRepository(GameContext context)
        {
            this.context = context;
        }

        public async Task<List<RoomModel>> GetFreeRooms() =>
            await dbSet.Where(x => x.State == GameState.AwaitingPlayers).ToListAsync();

        public async Task<RoomModel> GetRoom(Guid roomId) =>
            await dbSet.FirstOrDefaultAsync(x => x.RoomId == roomId);

        public void UpdateState(Guid roomId, GameState state)
        {
            var room = dbSet.Find(roomId);
            room.State = state;
            context.Entry(room).Property(x => x.State).IsModified = true;
        }

        public async Task CreateRoom(RoomModel room) => 
            await dbSet.AddAsync(room);

        public async Task SaveChanges() =>
            await context.SaveChangesAsync();
    }
}
