using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Life360
{
	public class Life360ViewModel : ApiResultBase
	{
		public string Response { get; set; }

		public Life360ViewModel(string response, ApiStatus status) : base(status)
		{
			this.Response = response;
		}
	}
}