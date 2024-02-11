namespace Blinkenlights.Dataschemas
{
	public class ModulePlacementData
	{
		public string Name { get; init; }
		public string Endpoint { get; init; }
		public int RefreshRateMs { get; init; }
		public int RowStart { get; init; }
		public int RowEnd { get; init; }
		public int ColStart { get; init; }
		public int ColEnd { get; init; }
	}
}
