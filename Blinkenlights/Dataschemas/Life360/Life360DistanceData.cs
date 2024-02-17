namespace Blinkenlights.Dataschemas
{
    public class Life360DistanceData
    {
        public string Distance { get; set; }

        public string TimeDelta { get; set; }

        public ApiStatus Status { get; set; }

        public static Life360DistanceData Clone(Life360DistanceData other, ApiStatus status)
        {
            return new Life360DistanceData()
            {
                Distance = other?.Distance,
                TimeDelta = other?.TimeDelta,
                Status = status
            };
        }
    }
}
