using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class Life360Data : IDatabaseData
    {
        public string Key() => typeof(Life360Data).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public List<Life360LocationData> Locations { get; init; }

        public Life360DistanceData DistanceData { get; init; }

        public static Life360Data Clone(Life360Data other, ApiStatus status)
        {
            return new Life360Data()
            {
                Status = status,
                TimeStamp = other?.TimeStamp,
                Locations = other?.Locations,
                DistanceData = other?.DistanceData
            };
        }
    }
}