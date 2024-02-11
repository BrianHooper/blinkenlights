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

		public IssTrackerData(ApiStatus status, string? filePath, double? latitude, double? longitude, DateTime? timeStamp)
		{
			Status = status;
			FilePath = filePath;
			Latitude = latitude;
			Longitude = longitude;
			TimeStamp = timeStamp;
		}

		public static IssTrackerData Clone(IssTrackerData other, ApiStatus status)
		{
			return new IssTrackerData(status, other?.FilePath, other?.Latitude, other?.Longitude, other?.TimeStamp);
		}
	}
}
