namespace Blinkenlights.Models.ViewModels.Life360
{
    public class Life360ViewModel
    {
        public Life360ViewModel(List<Life360Model> models)
        {
            Models = models;
        }

        public List<Life360Model> Models { get; set; }
    }

    public class Life360Model
    {
        public string Name { get; private set; }
        public string TimeStr { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }


        private Life360Model(string name, string timestamp, double latitude, double longitude)
        {
            Name = name;
            Longitude = longitude;
            Latitude = latitude;
            TimeStr = timestamp;
        }

        public static Life360Model Parse(string name, string timestamp, string latitudeStr, string longitudeStr)
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
            var diffMinutes = DateTime.Now.Subtract(lastSignalDateTime).Minutes;
            var diffMinutesStr = diffMinutes > 0 ? $"Diff: -{diffMinutes}" : string.Empty;

            var lastRefreshTimeStr = DateTime.Now.ToString("h:mm tt");
            var lastSignalTimeStr = lastSignalDateTime.ToString("h:mm tt");

            var fields = new string[]
            {
                $"Signal: {lastSignalTimeStr}",
                $"Refresh: {lastRefreshTimeStr}",
                diffMinutes > 0 ? $"Diff: -{diffMinutes}" : string.Empty
            }.Where(s => !string.IsNullOrWhiteSpace(s));

            var timeStr = string.Join(", ", fields);
            return new Life360Model(name, timeStr, latitude, longitude);
        }

        public static Life360Model Parse(Member member)
        {
            return Parse(member?.FirstName, member?.Location?.Timestamp?.ToString(), member?.Location?.Latitude?.ToString(), member?.Location?.Longitude?.ToString());
        }
    }
}
