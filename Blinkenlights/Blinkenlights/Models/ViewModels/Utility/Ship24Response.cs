namespace Blinkenlights.Models.ViewModels.Utility
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public partial class Ship24Response
	{
		[JsonProperty("data")]
		public Data Data { get; set; }

		public static Ship24Response Deserialize(string data)
		{
			try
			{
				var response = JsonConvert.DeserializeObject<Ship24Response>(data);
				return response;
			}
			catch 
			{
			}
			return default(Ship24Response);
		}
	}

	public partial class Data
	{
		[JsonProperty("trackings")]
		public List<Tracking> Trackings { get; set; }
	}

	public partial class Tracking
	{
		[JsonProperty("tracker")]
		public Tracker Tracker { get; set; }

		[JsonProperty("shipment")]
		public Shipment Shipment { get; set; }
	}

	public partial class Shipment
	{
		[JsonProperty("shipmentId")]
		public string ShipmentId { get; set; }

		[JsonProperty("statusCode")]
		public object StatusCode { get; set; }

		[JsonProperty("statusCategory")]
		public string StatusCategory { get; set; }

		[JsonProperty("statusMilestone")]
		public string StatusMilestone { get; set; }

		[JsonProperty("originCountryCode")]
		public string OriginCountryCode { get; set; }

		[JsonProperty("destinationCountryCode")]
		public string DestinationCountryCode { get; set; }

		[JsonProperty("delivery")]
		public Delivery Delivery { get; set; }
	}

	public partial class Delivery
	{
		[JsonProperty("estimatedDeliveryDate")]
		public object EstimatedDeliveryDate { get; set; }

		[JsonProperty("service")]
		public object Service { get; set; }

		[JsonProperty("signedBy")]
		public object SignedBy { get; set; }
	}

	public partial class Tracker
	{
		[JsonProperty("trackerId")]
		public string TrackerId { get; set; }

		[JsonProperty("trackingNumber")]
		public string TrackingNumber { get; set; }

		[JsonProperty("isSubscribed")]
		public bool IsSubscribed { get; set; }

		[JsonProperty("shipmentReference")]
		public object ShipmentReference { get; set; }

		[JsonProperty("createdAt")]
		public DateTime CreatedAt { get; set; }
	}
}
