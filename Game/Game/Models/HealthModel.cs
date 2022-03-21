namespace Game.Models
{
    public class HealthModel : JsonModel
    {
        public int Health { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
    }
}
