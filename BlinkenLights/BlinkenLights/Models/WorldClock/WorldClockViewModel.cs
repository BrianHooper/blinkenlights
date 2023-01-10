using Newtonsoft.Json.Linq;

namespace BlinkenLights.Models.WorldClock
{
    public class WorldClockViewModel
    {
        public Dictionary<string, int> TimeZoneInfos { get; set; }
    }
}
