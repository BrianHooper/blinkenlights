namespace Blinkenlights.Dataschemas
{
    public class FlightData
    {
        public string SourceAirport { get; set; }

        public string DestAirport { get; set; }

        public string FlightCode { get; set; }

        public string Airline { get; set; }

        public string PlaneType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double MapLeftPercentage { get; set; }

        public double MapTopPercentage { get; set; }

        public int Heading { get; set; }
    }
}