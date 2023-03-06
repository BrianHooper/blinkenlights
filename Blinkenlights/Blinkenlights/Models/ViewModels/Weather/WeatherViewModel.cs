using Blinkenlights.Models.Api.ApiResult;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Weather
{
	public class WeatherViewModel : ApiResultBase
	{
		public string Description { get; set; }

		public List<WeatherDayModel> DayModels { get; set; }

		public string GraphModel { get; set; }

		public List<CurrentCondition> CurrentConditions { get; set; }

		public WeatherViewModel(ApiStatus status) : base(status)
		{
		}

		public WeatherViewModel(string description, List<WeatherDayModel> dayModels, WeatherGraphModel graphModel, List<CurrentCondition> currentConditions, ApiStatus status) : base(status)
		{
			this.Description = description;
			this.DayModels = dayModels;
			this.CurrentConditions = currentConditions;
			this.GraphModel = JsonConvert.SerializeObject(graphModel);
		}
	}

	public class WeatherDayModel
	{
		public WeatherDayModel(string dayName, string icon, string report, List<WeatherData> points)
		{
			DayName = dayName;
			Icon = icon;
			Report = report;
			WeatherDataPoints = new WeatherDataPoints(points);
			WeatherDataPointsModel = JsonConvert.SerializeObject(WeatherDataPoints);
		}

		public string DayName { get; set; }

		public string Icon { get; set; }

		public string Report { get; set; }

		public WeatherDataPoints WeatherDataPoints { get; set; }

		public string WeatherDataPointsModel { get; set; }
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
