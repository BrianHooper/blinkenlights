using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Slideshow;
using LiteDbLibrary;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace Blinkenlights.Transformers
{
	public class SlideshowTransformer : TransformerBase
	{
		public SlideshowTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var astronomyResponse = GetAstronomyPicOfTheDay();

			var testFrame = new SlideshowFrame("Title", "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fb/Solar_System_true_color_%28captions%29.jpg/800px-Solar_System_true_color_%28captions%29.jpg", "");
			var frames = new List<SlideshowFrame>() { astronomyResponse.Result.Frame, testFrame }.Where(x => x != null).ToList();
			return new SlideshowViewModel(frames, astronomyResponse.Result.Status);
		}

		public async Task<(SlideshowFrame Frame, ApiStatus Status)> GetAstronomyPicOfTheDay()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.Astronomy);

			if (apiResponse is null)
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, "API Response is null"));
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, "API Response data is empty", apiResponse.LastUpdateTime));
			}

			if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, errorMessage, apiResponse.LastUpdateTime));
			}

			SlideshowJsonModel slideshowData;
			try
			{
				slideshowData = JsonConvert.DeserializeObject<SlideshowJsonModel>(apiResponse.Data);
			}
			catch (JsonException)
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, "Exception while deserializing API response", apiResponse.LastUpdateTime));
			}

			if (string.IsNullOrWhiteSpace(slideshowData?.Title)
				|| string.IsNullOrWhiteSpace(slideshowData?.Source)
				|| string.IsNullOrWhiteSpace(slideshowData?.Url))
			{
				return (null, ApiStatus.Failed(ApiType.Astronomy, "Missing required data from api", apiResponse.LastUpdateTime));
			}

			var status = ApiStatus.Success(ApiType.Astronomy, apiResponse);
			this.ApiHandler.TryUpdateCache(apiResponse);
			var frame = new SlideshowFrame($"NASA Picture of the Day: {slideshowData.Title}", slideshowData.Source, slideshowData.Url);
			return (frame, status);
		}
	}
}
