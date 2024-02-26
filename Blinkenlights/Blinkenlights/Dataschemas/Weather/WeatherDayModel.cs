using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class WeatherDayModel
    {
        public string DayName { get; set; }

        public WeatherDataModel[] WeatherData { get; init; }

        public string Icon { get; set; }

        public string Report { get; set; }

        public WeatherDayModel(string dayName, string icon, string report)
        {
            DayName = dayName;
            Icon = icon;
            Report = report;

            int size = 24;
            WeatherData = new WeatherDataModel[size];
            for (int i = 0; i < size; i++)
            {
                WeatherData[i] = new WeatherDataModel()
                {
                    xHour = i,
                    temperature = 0.0,
                    rain = 0.0,
                    humidity = 0.0
                };
            }
        }

        public string SerializePoints()
        {
            return JsonSerializer.Serialize(WeatherData);
        }
    }
}