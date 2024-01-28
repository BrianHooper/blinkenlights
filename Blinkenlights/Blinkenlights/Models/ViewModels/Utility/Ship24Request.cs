using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Utility
{
	public class Ship24Request
	{
		[JsonProperty("trackingNumber")]
		public string TrackingNumber { get; set; }

		[JsonProperty("destinationCountryCode")]
		public string DestinationCountryCode { get; set; }

		[JsonProperty("destinationPostCode")]
		public string DestinationPostCode { get; set; }

		public Ship24Request(string trackingNumber) 
		{
			this.TrackingNumber = trackingNumber;
			this.DestinationCountryCode = "US";
			this.DestinationPostCode = "98148";
		}
	}
}
