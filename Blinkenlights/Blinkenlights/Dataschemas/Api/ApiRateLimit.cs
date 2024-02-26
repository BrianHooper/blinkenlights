using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class ApiRateLimit : IDatabaseData
    {
        public string Key() => typeof(ApiRateLimit).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public Dictionary<string, List<DateTime>> ApiCalls { get; init; }

        public DateTime? TimeStamp { get; init; }

        public ApiRateLimit() 
        {
            this.ApiCalls = new Dictionary<string, List<DateTime>>();
        }
    }
}
