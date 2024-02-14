using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class WeatherData : IModuleData
    {
        public string Key() => typeof(WeatherData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public string Description { get; set; }

        public List<KeyValuePair<string, WeatherDayModel>> DayModels { get; set; }

        public WeatherGraphModel GraphModel { get; set; }

        public List<CurrentCondition> CurrentConditions { get; set; }

        public static WeatherData Clone(WeatherData other, ApiStatus status)
        {
            return new WeatherData()
            {
                Status = status,
                TimeStamp = other?.TimeStamp,
                Description = other?.Description,
                DayModels = other?.DayModels,
                GraphModel = other?.GraphModel,
                CurrentConditions = other?.CurrentConditions
            };
        }
    }


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

    public class WeatherGraphModel
    {
        public int yAxisMin { get; set; }

        public int yAxisMax { get; set; }
    }

    public class WeatherDataPoints
    {
        public List<WeatherDataModel> Points { get; set; }
    }

    public class WeatherDataModel
    {
        public int xHour { get; set; }
        public double temperature { get; set; }
        public double rain { get; set; }
        public double humidity { get; set; }
    }

    public class CurrentCondition
    {
        public string Key;

        public string Value;

        public string Icon;

        public string Description;

        public static CurrentCondition Create(string key, string value, string icon, string description)
        {
            return new CurrentCondition()
            {
                Key = key,
                Value = value,
                Icon = icon,
                Description = description
            };
        }
    }
}