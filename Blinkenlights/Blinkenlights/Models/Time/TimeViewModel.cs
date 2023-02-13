namespace BlinkenLights.Modules.Time
{
    public class TimeViewModel
    {
        public Dictionary<string, int> TimeZoneInfos { get; set; }

        public SortedDictionary<string, string> CountdownInfos { get; set; }

        public string Status { get; set; }
    }
}