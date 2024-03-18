using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.FlightStatus
{

	public class FlightRadarJsonModel
	{
		[JsonPropertyName("timestamp")]
		public string Timestamp { get; set; }

		[JsonPropertyName("flights")]
		public List<Flight> Flights { get; set; }
	}

	public class Flight
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("fid")]
        public string Fid { get; set; }

        [JsonPropertyName("latitude")]
		public double? Latitude { get; set; }

		[JsonPropertyName("longitude")]
		public double? Longitude { get; set; }

		[JsonPropertyName("altitude")]
		public int? Altitude { get; set; }

		[JsonPropertyName("aircraft")]
		public string Aircraft { get; set; }

		[JsonPropertyName("heading")]
		public int? Heading { get; set; }

		[JsonPropertyName("origin")]
		public string Origin { get; set; }

		[JsonPropertyName("destination")]
		public string Destination { get; set; }
	}
}
