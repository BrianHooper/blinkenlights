namespace Blinkenlights.Dataschemas
{
	public class PackageTrackingItem : ModuleDataBase
    {
		public int Id { get; set; }

		public string TrackingNumber { get; set; }

		public string Name { get; set; }

		public string Carrier { get; set; }

		public string Url { get; set; }

		public bool SameIndex(IModuleData other)
		{
			return string.Equals(this.TrackingNumber, (other as PackageTrackingItem).TrackingNumber);
		}
	}
}
