namespace BlinkenLights.Modules.WorldClock
{
    public class WorldClockViewModel
    {
        public Dictionary<string, int> TimeZoneInfos { get; set; }

        public SortedDictionary<string, string> CountdownInfos { get; set; }
    }
}
