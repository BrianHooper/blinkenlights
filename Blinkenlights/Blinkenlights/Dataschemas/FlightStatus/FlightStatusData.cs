using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class FlightStatusData : IDatabaseData
    {
        public string Key() => typeof(FlightStatusData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public List<FlightData> Flights { get; init; }

        public static FlightStatusData Clone(FlightStatusData other, ApiStatus status)
        {
            return new FlightStatusData()
            {
                Status = status,
                TimeStamp = other?.TimeStamp,
                Flights = other?.Flights,
            };
        }
    }
}