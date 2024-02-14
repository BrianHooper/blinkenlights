using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class TimeData : IModuleData
    {
        public string Key() => typeof(TimeData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public Dictionary<string, int> TimeZoneInfos { get; set; }

        public SortedDictionary<string, string> CountdownInfos { get; set; }

        public static TimeData Clone(TimeData other, ApiStatus status)
        {
            return new TimeData()
            {
                Status = status,
                TimeStamp = other?.TimeStamp
            };
        }
    }
}