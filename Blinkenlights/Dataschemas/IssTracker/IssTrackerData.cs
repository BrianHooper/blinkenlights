using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class IssTrackerData : IModuleData
    {
        public string Key() => typeof(IssTrackerData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public ApiStatus Status { get; init; }

        public string? FilePath { get; init; }

        public double? Latitude { get; init; }

        public double? Longitude { get; init; }

        public DateTime? TimeStamp { get; init; }

        public static IssTrackerData Clone(IssTrackerData other, ApiStatus status)
        {
            return new IssTrackerData()
            {
                Status = status,
                FilePath = other?.FilePath,
                Latitude = other?.Latitude,
                Longitude = other?.Longitude,
                TimeStamp = other?.TimeStamp
            };
        }
    }
}
