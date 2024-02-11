using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
	public class IndexModuleData : IModuleData
	{
		public string Key() => typeof(IssTrackerData).Name;

		public string Value() => JsonSerializer.Serialize(this);


	}
}
