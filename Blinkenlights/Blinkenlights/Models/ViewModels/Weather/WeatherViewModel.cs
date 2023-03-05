using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Weather
{
	public class WeatherViewModel : ApiResultBase
	{
		public string Response { get; set; }

		public WeatherViewModel(string response, ApiStatus status) : base(status)
		{
			this.Response = response;
		}
	}
}
