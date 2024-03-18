namespace Blinkenlights.Dataschemas
{
    public class Life360DistanceData
    {
        public string Distance { get; set; }

        public int TimeDelta { get; set; }

        public string Time { get; set; }

        public static Life360DistanceData Clone(Life360DistanceData other)
        {
            return new Life360DistanceData()
            {
                Distance = other?.Distance,
                TimeDelta = other?.TimeDelta ?? 0,
                Time = other?.Time,
            };
        }
    }
}
