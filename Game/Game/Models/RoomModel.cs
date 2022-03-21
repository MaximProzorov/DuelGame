namespace Game.Models
{
    public class RoomModel
    {
        public RoomModel()
        {
            Users = new HashSet<UserModel>();
        }
        public Guid RoomId { get; set; }
        public GameState State { get; set; }
        public ICollection<UserModel> Users { get; set; }
    }
}
