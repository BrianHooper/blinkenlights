using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class WWIIData : IDatabaseData
    {
        public string Key() => typeof(WWIIData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public Dictionary<string, WWIIDayData> Days { get; init; }

        public static WWIIData Clone(WWIIData other, ApiStatus status)
        {
            return new WWIIData()
            {
                Status = status,
                Days = other?.Days,
                TimeStamp = other?.TimeStamp
            };
        }
    }
}