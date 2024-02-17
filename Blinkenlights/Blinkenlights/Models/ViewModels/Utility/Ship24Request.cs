using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Utility
{
    public class Ship24Request
    {
        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonPropertyName("destinationCountryCode")]
        public string DestinationCountryCode { get; set; }

        [JsonPropertyName("destinationPostCode")]
        public string DestinationPostCode { get; set; }

        public Ship24Request(string trackingNumber)
        {
            this.TrackingNumber = trackingNumber;
            this.DestinationCountryCode = "US";
            this.DestinationPostCode = "98148";
        }
    }
}
