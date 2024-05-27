namespace Blinkenlights.Dataschemas
{
    public class FlightData
    {
        public string Fid { get; set; }

        public string SourceAirport { get; set; }

        public string DestAirport { get; set; }

        public string FlightCode { get; set; }

        public string AircraftType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int MapLeftPercentage { get; set; }

        public int MapTopPercentage { get; set; }

        public int Heading { get; set; }

        public string Color { get; set; }

        public SingleFlightData SingleFlightData { get; set;}
    }
}