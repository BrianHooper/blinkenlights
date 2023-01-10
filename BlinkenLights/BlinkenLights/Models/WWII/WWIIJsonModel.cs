using Newtonsoft.Json.Linq;

namespace BlinkenLights.Models.WWII
{
    public class WWIIJsonModel
    {
        public Dictionary<string, WWIIDayJsonModel> Days { get; set; }
    }
}
