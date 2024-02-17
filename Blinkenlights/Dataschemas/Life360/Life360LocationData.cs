namespace Blinkenlights.Dataschemas
{
    public class Life360LocationData
    {
        public string Name { get; init; }
        public string TimeStr { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public DateTime Time { get; init; }
        public int TimeDeltaSeconds { get; init; }
        public string TimeDeltaStr { get; init; }

        public static Life360LocationData Parse(string name, string timestamp, string latitudeStr, string longitudeStr)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(timestamp) || !double.TryParse(timestamp, out var epoch))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(latitudeStr) || !double.TryParse(latitudeStr, out var latitude))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(longitudeStr) || !double.TryParse(longitudeStr, out var longitude))
            {
                return null;
            }

            var offset = DateTimeOffset.Now.Offset;
            var lastSignalDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).Add(offset);

            var deltaSecondsTotal = (int)(DateTime.Now - lastSignalDateTime).TotalSeconds;
            var minutes = (deltaSecondsTotal / 60).ToString("D2");
            var seconds = (deltaSecondsTotal % 60).ToString("D2");
            var timeDeltaStr = $"-{minutes}:{seconds}";

            var lastRefreshTimeStr = DateTime.Now.ToString("h:mm tt");
            var lastSignalTimeStr = lastSignalDateTime.ToString("h:mm tt");

            var fields = new string[]
            {
                $"Signal: {lastSignalTimeStr}",
                $"Refresh: {lastRefreshTimeStr}",
                $"Diff: {timeDeltaStr}"
            }.Where(s => !string.IsNullOrWhiteSpace(s));

            var timeStr = string.Join(", ", fields);
            return new Life360LocationData()
            {
                Latitude = latitude,
                Longitude = longitude,
                Name = name,
                TimeStr = timeStr,
                Time = lastSignalDateTime,
                TimeDeltaSeconds = deltaSecondsTotal,
                TimeDeltaStr = timeDeltaStr
            };
        }
    }
}