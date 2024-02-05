using Blinkenlights.Models.Api.ApiResult;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Weather
{
	public class WeatherViewModel : ApiResultBase
	{
		public string Description { get; set; }

		public List<KeyValuePair<string, WeatherDayModel>> DayModels { get; set; }

		public string GraphModel { get; set; }

		public List<CurrentCondition> CurrentConditions { get; set; }

		public WeatherViewModel(ApiStatus status) : base("Weather", status)
		{
		}

		public WeatherViewModel(string description, List<KeyValuePair<string, WeatherDayModel>> dayModels, WeatherGraphModel graphModel, List<CurrentCondition> currentConditions, ApiStatus status) : base("Weather", status)
		{
			this.Description = description;
			this.DayModels = dayModels;
			this.CurrentConditions = currentConditions;
			this.GraphModel = JsonConvert.SerializeObject(graphModel);
		}
	}

	public class WeatherDayModel
	{
		public string DayName { get; set; }

		public WeatherData[] WeatherData { get; init; }

		public string Icon { get; set; }

		public string Report { get; set; }

		public WeatherDayModel(string dayName, string icon, string report)
		{
			DayName = dayName;
			Icon = icon;
			Report = report;

			int size = 24;
			WeatherData = new WeatherData[size];
			for (int i = 0; i < size; i++)
			{
				WeatherData[i] = new WeatherData(i, 0.0, 0.0, 0.0);
			}
		}

		public string SerializePoints()
		{
			var jsonSettings = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Include,
			};
			return JsonConvert.SerializeObject(WeatherData, jsonSettings);
		}
	}

	public class WeatherGraphModel
	{
		public WeatherGraphModel(int yAxisMin, int yAxisMax)
		{
			this.yAxisMin = yAxisMin;
			this.yAxisMax = yAxisMax;
		}

		public int yAxisMin { get; set; }

		public int yAxisMax { get; set; }
	}

	public class WeatherDataPoints
	{
		public WeatherDataPoints(List<WeatherData> points)
		{
			Points = points;
		}

		public List<WeatherData> Points { get; set; }
	}

	public class WeatherData
	{
		public WeatherData(int xHour, double temperature, double rain, double humidity)
		{
			this.xHour = xHour;
			this.temperature = temperature;
			this.rain = rain;
			this.humidity = humidity;
		}

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

		public CurrentCondition(string key, string value, string icon, string description)
		{
			Key = key;
			Value = value;
			Icon = icon;
			Description = description;
		}
	}
}
