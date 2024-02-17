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
}