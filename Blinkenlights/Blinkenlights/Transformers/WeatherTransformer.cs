using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;
using Blinkenlights.Models.ViewModels.Weather;
using Blinkenlights.Utilities;
using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;

namespace Blinkenlights.Transformers
{
	public class WeatherTransformer : TransformerBase
	{
		public WeatherTransformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public async override Task<IModuleViewModel> Transform()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.VisualCrossingWeather);

			if (apiResponse is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.VisualCrossingWeather, null, "API Response is null");
				return new WeatherViewModel(errorStatus);
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.VisualCrossingWeather, apiResponse, "API Response data is empty");
				return new WeatherViewModel(errorStatus);
			}

			WeatherJsonModel weatherJsonModel;
			try
			{
				weatherJsonModel = WeatherJsonModel.FromJson(apiResponse.Data);
			}
			catch (Exception)
			{
				var errorStatus = ApiStatus.Failed(ApiType.VisualCrossingWeather, apiResponse, "Deserialization error");
				return new WeatherViewModel(errorStatus);
			}

			if (weatherJsonModel?.Days?.Any() != true)
			{
				var errorStatus = ApiStatus.Failed(ApiType.VisualCrossingWeather, apiResponse, "Deserialized response is empty");
				return new WeatherViewModel(errorStatus);
			}

			var jsonDaysModel = weatherJsonModel.Days.Take(5);

			var hoursFlattened = jsonDaysModel.SelectMany(d => d.Hours);
			var temperatureMin = hoursFlattened.Min(h => h.Temp);
			var temperatureMax = hoursFlattened.Max(h => h.Temp);
			var yAxisMin = temperatureMin <= 0 ? (int)temperatureMin - 10 : 0;
			var yAxisMax = temperatureMin >= 100 ? (int)temperatureMin + 10 : 100;
			var graphModel = new WeatherGraphModel(yAxisMin, yAxisMax);

			var dayModels = new List<WeatherDayModel>();
			foreach(var day in jsonDaysModel)
			{
				if (day?.Hours?.Any() != true)
				{
					continue;
				}

				var points = new List<WeatherData>();
				foreach (var hourModel in day.Hours)
				{
					if (hourModel == null)
					{
						continue;
					}
					var dateTimePoint = Helpers.FromEpoch(hourModel.DatetimeEpoch, true, true);
					var xHour = dateTimePoint.Hour;

					var precipProb = (dateTimePoint >= DateTime.Now && hourModel.Precipprob > 0) ? hourModel.Precipprob : -1;
					var weatherDataPoint = new WeatherData(xHour, hourModel.Temp, precipProb, hourModel.Humidity);
					points.Add(weatherDataPoint);
				}
				var dayOfWeek = Helpers.FromEpoch(day.DatetimeEpoch, true, true).DayOfWeek.ToString();
				var dayModel = new WeatherDayModel(dayOfWeek, day.Icon.ToString(), day.Description, points);
				dayModels.Add(dayModel);
			}

			var lowTemp = dayModels.FirstOrDefault().WeatherDataPoints.Points.Min(p => p.temperature);
			var highTemp = dayModels.FirstOrDefault().WeatherDataPoints.Points.Max(p => p.temperature);

			var currentConditions = new List<CurrentCondition>
			{
				new CurrentCondition("Chance Rain", AsPercent(weatherJsonModel.CurrentConditions.Precipprob), "chance_rain.png", "Percent chance of rain"),
				new CurrentCondition("Precipitation", AsPrecipitation(weatherJsonModel.CurrentConditions.Precip), "icons8-rainwater-catchment-48.png", "Expected rainfall in inches"),

				new CurrentCondition("Temperature", AsTemperature(weatherJsonModel.CurrentConditions.Temp), "hot.png", "Current temperature (F)"),
				new CurrentCondition("UV Index", weatherJsonModel.CurrentConditions.Uvindex.ToString(), "icons8-sunlight-100.png", "Current UV index - 0-2 (low), 3-5 (moderate), 6-7 (high), 8+ (severe)"),

				new CurrentCondition("Low Temp", AsTemperature(lowTemp), "low-temperature.png", "Today's low temperature (F)"),
				new CurrentCondition("High Temp", AsTemperature(highTemp), "high-temperature.png", "Today's high temperature (F)"),

				new CurrentCondition("Humidity", AsPercent(weatherJsonModel.CurrentConditions.Humidity), "icons8-moisture-40.png", "Humidity"),
				new CurrentCondition("Cloud Cover", AsPercent(weatherJsonModel.CurrentConditions.Cloudcover), "cloudy.png", "Current Cloud Cover Percent"),

				new CurrentCondition("Moon Phase", AsPercent(weatherJsonModel.CurrentConditions.Moonphase, true), "moon-phase.png", "Moon Phase Percent"),
				new CurrentCondition("Sunset", AsTime(weatherJsonModel.CurrentConditions.SunsetEpoch), "sunset.png", "Sunset Time"),

				new CurrentCondition("Wind Speed", AsSpeed(weatherJsonModel.CurrentConditions.Windspeed), "windy.png", "Wind Speed"),
				new CurrentCondition("Wind Direction", AsDirection(weatherJsonModel.CurrentConditions.Winddir), "windsock.png", $"Wind Direction: {weatherJsonModel.CurrentConditions.Winddir} degrees"),
			};

			var status = ApiStatus.Success(ApiType.VisualCrossingWeather, apiResponse);
			this.ApiHandler.TryUpdateCache(apiResponse);
			var viewModel = new WeatherViewModel(weatherJsonModel.Description, dayModels, graphModel, currentConditions, status);
			return viewModel;
		}

		private string AsPrecipitation(double precip)
		{
			return ((int)precip).ToString() + "\"";
		}

		private string AsSpeed(double windspeed)
		{
			return ((int)windspeed).ToString() + "mph";
		}

		private string AsTemperature(double temp)
		{
			return ((int)temp).ToString() + "°";
		}

		private string AsPercent(double? percent, bool multiply = false)
		{
			if (percent == null)
			{
				return string.Empty;
			}

			return ((int) (multiply ? percent * 100 : percent)).ToString() + "%";
		}

		private string AsDirection(double degrees)
		{
			string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
			var test = caridnals[(int)Math.Round(((double)degrees * 10 % 3600) / 225)];
			return test;
		}

		private string AsTime(long? sunsetEpoch)
		{
			return sunsetEpoch != null ? Helpers.FromEpoch(sunsetEpoch.Value, true, true).ToString("hh:mm") : string.Empty;
		}
	}
}
