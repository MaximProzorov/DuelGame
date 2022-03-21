namespace Game.Models
{
    public class FinishModel : JsonModel
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
    }
}
