using System.Text.Json;

namespace Game.Models
{
    public abstract class JsonModel
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize((object)this);
        }
    }
}
