using Blinkenlights.Dataschemas;
using System.Text.Json;

namespace Blinkenlights.Models.ViewModels.Weather
{
    public class WeatherViewModel : ModuleViewModelBase
    {
        public string Description { get; set; }

        public List<KeyValuePair<string, WeatherDayModel>> DayModels { get; set; }

        public string GraphModel { get; set; }

        public List<CurrentCondition> CurrentConditions { get; set; }

		public WeatherViewModel() : base("Weather")
		{
		}

		public WeatherViewModel(ApiStatus status) : base("Weather", status)
		{
		}

		public WeatherViewModel(string description, List<KeyValuePair<string, WeatherDayModel>> dayModels, WeatherGraphModel graphModel, List<CurrentCondition> currentConditions, ApiStatus status) : base("Weather", status)
        {
            this.Description = description;
            this.DayModels = dayModels;
            this.CurrentConditions = currentConditions;
            this.GraphModel = JsonSerializer.Serialize(graphModel);
        }
    }
}
