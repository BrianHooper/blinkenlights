using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;
using Blinkenlights.Models.ViewModels.Weather;
using Google.Apis.Calendar.v3.Data;

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
				return new WeatherViewModel(null, errorStatus);
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.VisualCrossingWeather, apiResponse, "API Response data is empty");
				return new WeatherViewModel(null, errorStatus);
			}

			var status = ApiStatus.Success(ApiType.VisualCrossingWeather, apiResponse);
			this.ApiHandler.TryUpdateCache(apiResponse);
			return new WeatherViewModel(apiResponse.Data, status);
		}
	}
}
