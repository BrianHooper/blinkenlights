using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Slideshow;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
	public class SlideshowTransformer : TransformerBase
	{
		public SlideshowTransformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public async override Task<IModuleViewModel> Transform()
		{
			var astronomyResponse = await GetAstronomyPicOfTheDay();

			var testFrame = new SlideshowFrame("Title", "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fb/Solar_System_true_color_%28captions%29.jpg/800px-Solar_System_true_color_%28captions%29.jpg", "");
			var frames = new List<SlideshowFrame>() { astronomyResponse.Frame, testFrame }.Where(x => x != null).ToList();
			return new SlideshowViewModel(frames, astronomyResponse.Status);
		}

		public async Task<(SlideshowFrame Frame, ApiStatus Status)> GetAstronomyPicOfTheDay()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.Astronomy);

			if (apiResponse is null)
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, null, "API Response is null"));
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, apiResponse, "API Response data is empty"));
			}

			if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, apiResponse, errorMessage));
			}

			SlideshowJsonModel slideshowData;
			try
			{
				slideshowData = JsonConvert.DeserializeObject<SlideshowJsonModel>(apiResponse.Data);
			}
			catch (JsonException)
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, apiResponse, "Exception while deserializing API response"));
			}

			if (string.IsNullOrWhiteSpace(slideshowData?.Title)
				|| string.IsNullOrWhiteSpace(slideshowData?.Source)
				|| string.IsNullOrWhiteSpace(slideshowData?.Url))
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, apiResponse, "Missing required data from api"));
			}

			var status = ApiStatus.Success(ApiType.Astronomy, apiResponse);
			this.ApiHandler.TryUpdateCache(apiResponse);
			var frame = new SlideshowFrame($"NASA Picture of the Day: {slideshowData.Title}", slideshowData.Source, slideshowData.Url);
			return (frame, status);
		}
	}
}
