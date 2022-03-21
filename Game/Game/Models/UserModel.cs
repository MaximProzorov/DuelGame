using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game.Models
{
    public class UserModel
    {
        public Guid PlayerId { get; set; }
        public string Username { get; set; }
        public Guid? RoomId { get; set; }
        public RoomModel? Room { get; set; }
    }
}
