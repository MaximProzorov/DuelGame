using Game.Models;
using Microsoft.EntityFrameworkCore;

namespace Game.Data.Repositories
{
    public interface IUserRepository
    {
        Task<UserModel> GetUser(Guid playerId);
        Task<UserModel> GetUserByName(string username);
        Task CreateUser(UserModel user);
        Task SaveChanges();
    }
    public class UserRepository : IUserRepository
    {
        private readonly DbContext context;
        private DbSet<UserModel> dbSet => context.Set<UserModel>();

        public UserRepository(GameContext context)
        {
            this.context = context;
        }

        public async Task<UserModel> GetUser(Guid playerId) =>
            await dbSet.FirstOrDefaultAsync(x => x.PlayerId == playerId);

        public async Task<UserModel> GetUserByName(string username) =>
            await dbSet.FirstOrDefaultAsync(x => x.Username == username);

        public async Task CreateUser(UserModel user) => 
            await dbSet.AddAsync(user);

        public async Task SaveChanges() =>
            await context.SaveChangesAsync();
    }
}
