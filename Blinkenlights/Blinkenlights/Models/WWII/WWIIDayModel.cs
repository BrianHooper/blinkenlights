using Blinkenlights.Models.ApiResult;

namespace BlinkenLights.Models.WWII
{
    public class WWIIDayModel : ApiResultBase
    {
        public WWIIDayModel(string date, List<string> globalEvents, List<KeyValuePair<string, List<string>>> regionalEvents, ApiStatus status): base(status)
        {
            Date = date;
            GlobalEvents = globalEvents;
            RegionalEvents = regionalEvents;
        }

        public string Date { get; set; }
        public List<string> GlobalEvents { get; set; }
        public List<KeyValuePair<string, List<string>>> RegionalEvents { get; set; }
    }
}
