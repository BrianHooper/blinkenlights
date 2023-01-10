using Newtonsoft.Json.Linq;

namespace BlinkenLights.Models.WWII
{
    public class WWIIDayJsonModel
    {
        public string DateStr { get; set; }

        public Dictionary<string, List<string>> Events { get; set; }
    }
}
