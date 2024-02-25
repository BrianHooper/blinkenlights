using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
	public class OuterSpaceData : IDatabaseData
	{
		public string Key() => typeof(OuterSpaceData).Name;

		public string Value() => JsonSerializer.Serialize(this);

		public DateTime? TimeStamp { get; init; }

		public IssTrackerData IssTrackerData { get; init; }

		public static OuterSpaceData Clone(OuterSpaceData other)
		{
			return new OuterSpaceData()
			{
				TimeStamp = other?.TimeStamp,
				IssTrackerData = other?.IssTrackerData,
			};
		}
	}
}
