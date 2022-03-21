using Microsoft.AspNetCore.SignalR;

namespace Game.Hubs
{
    public interface IPlayerHub
    {
        Task SendMessage(string user, string message);
    }
    public class PlayerHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
