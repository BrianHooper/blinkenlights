namespace BlinkenLights.Models.WWII
{
    public class WWIIDayModel
    {
        public string Date { get; set; }
        public List<string> GlobalEvents { get; set; }
        public List<KeyValuePair<string, List<string>>> RegionalEvents { get; set; }
        public string Status { get; set; }
    }
}
