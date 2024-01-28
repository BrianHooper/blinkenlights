using System.Text.Json.Serialization;

namespace LiteDbLibrary.Schemas
{
	public class PackageTrackingItem : ILiteDbObject
	{
		public int Id { get; set; }

		public string TrackingNumber { get; set; }

		public string Name { get; set; }

		public bool SameIndex(ILiteDbObject other)
		{
			return string.Equals(this.TrackingNumber, (other as PackageTrackingItem).TrackingNumber);
		}
	}
}
