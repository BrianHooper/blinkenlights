namespace Blinkenlights.Dataschemas
{
    public class WWIIDayData
    {
        public WWIIDayData(string date, List<string> globalEvents, List<KeyValuePair<string, List<string>>> regionalEvents)
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
