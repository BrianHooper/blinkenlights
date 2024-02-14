using Blinkenlights.Models.Api.ApiHandler;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Utility
{
    public class Ship24Cache
    {
        public DateTime LastUpdateTime { get; init; }

        public Dictionary<string, Ship24Response> Responses { get; set; }

        public Ship24Cache()
        {
            LastUpdateTime = DateTime.Now;
            Responses = new Dictionary<string, Ship24Response>();
        }

        public Ship24Cache(Dictionary<string, Ship24Response> responses, DateTime lastUpdateTime)
        {
            LastUpdateTime = lastUpdateTime;
            Responses = responses; //TODO filter by api timeout
        }

        public static Ship24Cache Deserialize(ApiResponse response)
        {

            if (string.IsNullOrWhiteSpace(response?.Data))
            {
                return new Ship24Cache();
            }

            try
            {
                var cache = JsonConvert.DeserializeObject<Dictionary<string, Ship24Response>>(response.Data);
                return new Ship24Cache(cache, response.LastUpdateTime);
            }
            catch
            {
            }

            return new Ship24Cache();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(Responses);
        }
    }
}
