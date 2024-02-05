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

		[JsonProperty("events")]
		public List<Event> Events { get; set; }
	}

	public partial class Shipment
	{
		[JsonProperty("shipmentId")]
		public string ShipmentId { get; set; }

		[JsonProperty("statusCode")]
		public string StatusCode { get; set; }

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
		public string EstimatedDeliveryDate { get; set; }

		[JsonProperty("service")]
		public string Service { get; set; }

		[JsonProperty("signedBy")]
		public string SignedBy { get; set; }
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
		public string ShipmentReference { get; set; }

		[JsonProperty("createdAt")]
		public string CreatedAt { get; set; }
	}

	public partial class Event
	{
		[JsonProperty("eventId")]
		public string EventId { get; set; }

		[JsonProperty("trackingNumber")]
		public string TrackingNumber { get; set; }

		[JsonProperty("eventTrackingNumber")]
		public string EventTrackingNumber { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("occurrenceDatetime")]
		public string OccurrenceDatetime { get; set; }

		[JsonProperty("order")]
		public string Order { get; set; }

		[JsonProperty("datetime")]
		public string Datetime { get; set; }

		[JsonProperty("hasNoTime")]
		public bool HasNoTime { get; set; }

		[JsonProperty("utcOffset")]
		public string UtcOffset { get; set; }

		[JsonProperty("location")]
		public string Location { get; set; }

		[JsonProperty("sourceCode")]
		public string SourceCode { get; set; }

		[JsonProperty("courierCode")]
		public string CourierCode { get; set; }

		[JsonProperty("statusCode")]
		public string StatusCode { get; set; }

		[JsonProperty("statusCategory")]
		public string StatusCategory { get; set; }

		[JsonProperty("statusMilestone")]
		public string StatusMilestone { get; set; }
	}
}
