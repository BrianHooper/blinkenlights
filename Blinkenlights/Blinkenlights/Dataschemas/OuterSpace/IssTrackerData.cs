namespace Blinkenlights.Dataschemas
{
    public class IssTrackerData
    {
        public ApiStatus Status { get; init; }

		public string? ImageData { get; init; }

		public double? Latitude { get; init; }

        public double? Longitude { get; init; }

        public static IssTrackerData Clone(IssTrackerData other, ApiStatus status)
        {
            return new IssTrackerData()
            {
                Status = status,
                Latitude = other?.Latitude,
                Longitude = other?.Longitude,
                ImageData = other?.ImageData
            };
        }
    }
}
