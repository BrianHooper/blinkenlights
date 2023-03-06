using Blinkenlights.Models.Api.ApiResult;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Astronomy
{
	public class AstronomyViewModel : ApiResultBase
	{
		public AstronomyViewModel(string title, string source, string url, ApiStatus status): base(status)
		{
			Title = title;
			Source = source;
			Url = url;
		}

		public AstronomyViewModel(ApiStatus status) : base(status)
		{
		}

		public string Title { get; set; }

		public string Source { get; set; }

		public string Url { get; set; }
	}
}
