namespace Blinkenlights.Models.ViewModels.Life360
{

	public partial class Life360JsonModel
	{
		public List<Member> Members { get; set; }
	}

	public partial class Member
	{
		public Features Features { get; set; }
		public Issues Issues { get; set; }
		public Location Location { get; set; }
		public List<Communication> Communications { get; set; }
		public object Medical { get; set; }
		public object Relation { get; set; }
		public long? CreatedAt { get; set; }
		public object Activity { get; set; }
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public long? IsAdmin { get; set; }
		public Uri Avatar { get; set; }
		public object PinNumber { get; set; }
		public string LoginEmail { get; set; }
		public string LoginPhone { get; set; }
	}

	public partial class Communication
	{
		public string Channel { get; set; }
		public string Value { get; set; }
		public string Type { get; set; }
	}

	public partial class Features
	{
		public long? Device { get; set; }
		public long? Smartphone { get; set; }
		public long? NonSmartphoneLocating { get; set; }
		public long? Geofencing { get; set; }
		public long? ShareLocation { get; set; }
		public object ShareOffTimestamp { get; set; }
		public long? Disconnected { get; set; }
		public long? PendingInvite { get; set; }
		public long? MapDisplay { get; set; }
	}

	public partial class Issues
	{
		public long? Disconnected { get; set; }
		public object Type { get; set; }
		public object Status { get; set; }
		public object Title { get; set; }
		public object Dialog { get; set; }
		public object Action { get; set; }
		public long? Troubleshooting { get; set; }
	}

	public partial class Location
	{
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string Accuracy { get; set; }
		public long? StartTimestamp { get; set; }
		public long? EndTimestamp { get; set; }
		public long? Since { get; set; }
		public long? Timestamp { get; set; }
		public string Name { get; set; }
		public object PlaceType { get; set; }
		public string Source { get; set; }
		public string SourceId { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string ShortAddress { get; set; }
		public long? InTransit { get; set; }
		public object TripId { get; set; }
		public object DriveSdkStatus { get; set; }
		//public long? Battery { get; set; }
		public long? Charge { get; set; }
		public long? WifiState { get; set; }
		public double? Speed { get; set; }
		public long? IsDriving { get; set; }
		public object UserActivity { get; set; }
	}
}
