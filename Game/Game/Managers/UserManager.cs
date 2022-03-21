using Game.Data.Repositories;

namespace Game.Managers
{
    public interface IUserManager
    {
        Task<Guid?> GetPlayerId(string username);
    }
    public class UserManager : IUserManager
    {
        private readonly IUserRepository repository;

        public UserManager(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Guid?> GetPlayerId(string username)
        {
            if(string.IsNullOrEmpty(username))
                return null;
            var user = await repository.GetUserByName(username);
            if(user == null)
            {
                user = new Models.UserModel
                {
                    PlayerId = Guid.NewGuid(),
                    Username = username
                };
                await repository.CreateUser(user);
                await repository.SaveChanges();
            }

            return user.PlayerId;
        }
    }
}
