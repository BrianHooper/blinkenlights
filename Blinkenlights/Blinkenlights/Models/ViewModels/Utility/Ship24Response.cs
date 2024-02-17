using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Utility
{

    public partial class Ship24Response
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; }

        public static Ship24Response Deserialize(string data)
        {
            try
            {
                var response = JsonSerializer.Deserialize<Ship24Response>(data);
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
        [JsonPropertyName("trackings")]
        public List<Tracking> Trackings { get; set; }
    }

    public partial class Tracking
    {
        [JsonPropertyName("tracker")]
        public Tracker Tracker { get; set; }

        [JsonPropertyName("shipment")]
        public Shipment Shipment { get; set; }

        [JsonPropertyName("events")]
        public List<Event> Events { get; set; }
    }

    public partial class Shipment
    {
        [JsonPropertyName("shipmentId")]
        public string ShipmentId { get; set; }

        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; }

        [JsonPropertyName("statusCategory")]
        public string StatusCategory { get; set; }

        [JsonPropertyName("statusMilestone")]
        public string StatusMilestone { get; set; }

        [JsonPropertyName("originCountryCode")]
        public string OriginCountryCode { get; set; }

        [JsonPropertyName("destinationCountryCode")]
        public string DestinationCountryCode { get; set; }

        [JsonPropertyName("delivery")]
        public Delivery Delivery { get; set; }
    }

    public partial class Delivery
    {
        [JsonPropertyName("estimatedDeliveryDate")]
        public string EstimatedDeliveryDate { get; set; }

        [JsonPropertyName("service")]
        public string Service { get; set; }

        [JsonPropertyName("signedBy")]
        public string SignedBy { get; set; }
    }

    public partial class Tracker
    {
        [JsonPropertyName("trackerId")]
        public string TrackerId { get; set; }

        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonPropertyName("isSubscribed")]
        public bool IsSubscribed { get; set; }

        [JsonPropertyName("shipmentReference")]
        public string ShipmentReference { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }
    }

    public partial class Event
    {
        [JsonPropertyName("eventId")]
        public string EventId { get; set; }

        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonPropertyName("eventTrackingNumber")]
        public string EventTrackingNumber { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("occurrenceDatetime")]
        public string OccurrenceDatetime { get; set; }

        [JsonPropertyName("order")]
        public string Order { get; set; }

        [JsonPropertyName("datetime")]
        public string Datetime { get; set; }

        [JsonPropertyName("hasNoTime")]
        public bool HasNoTime { get; set; }

        [JsonPropertyName("utcOffset")]
        public string UtcOffset { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("sourceCode")]
        public string SourceCode { get; set; }

        [JsonPropertyName("courierCode")]
        public string CourierCode { get; set; }

        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; }

        [JsonPropertyName("statusCategory")]
        public string StatusCategory { get; set; }

        [JsonPropertyName("statusMilestone")]
        public string StatusMilestone { get; set; }
    }
}
