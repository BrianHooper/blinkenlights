using System.Text.Json.Serialization;

namespace Blinkenlights.Dataschemas
{
    public class SingleFlightData
    {
        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        [JsonPropertyName("destination")]
        public string Destination { get; set; }

        [JsonPropertyName("departureRelative")]
        public string DepartureRelative { get; set; }

        [JsonPropertyName("arrivalRelative")]
        public string ArrivalRelative { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; }

        [JsonPropertyName("aircraft")]
        public string Aircraft { get; set; }

        [JsonPropertyName("airline")]
        public string Airline { get; set; }

        [JsonPropertyName("fnia")]
        public string Fnia { get; set; }
    }
}
