using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.Slideshow;
using Newtonsoft.Json;

namespace Blinkenlights.DataFetchers
{
    public class SlideshowDataFetcher : DataFetcherBase<SlideshowData>
    {
        public SlideshowDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromHours(4), databaseHandler, apiHandler)
        {
            Start();
        }

        protected override SlideshowData GetRemoteData(SlideshowData existingData = null)
		{
			var apod = GetPageParsePicOfTheDay(ApiType.Astronomy, "NASA Picture of the Day");
			var wiki = GetPageParsePicOfTheDay(ApiType.WikiPotd, "Wikipedia Picture of the Day");

			// NASA POTD https://apod.nasa.gov/apod/astropix.html
			// WIKIPEDIA POTD https://en.wikipedia.org/wiki/Wikipedia:Picture_of_the_day
			// VOA https://www.voanews.com/p/5341.html
			return new SlideshowData()
            {
                TimeStamp = DateTime.Now,
                Frames = new List<SlideshowFrame>()
                {
                    apod.Result,
                    wiki.Result,
                }
            };
        }

        public async Task<SlideshowFrame> GetPageParsePicOfTheDay(ApiType apiType, string title)
        {
            var apiResponse = await this.ApiHandler.Fetch(apiType);

            if (apiResponse is null)
            {
                return new SlideshowFrame(ApiStatus.Failed(apiType.ToString(), "API Response is null"));
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                return new SlideshowFrame(ApiStatus.Failed(apiType.ToString(), "API Response data is empty", apiResponse.LastUpdateTime));
            }

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
            {
                return new SlideshowFrame(ApiStatus.Failed(apiType.ToString(), errorMessage, apiResponse.LastUpdateTime));
            }

            SlideshowJsonModel slideshowData;
            try
            {
                slideshowData = JsonConvert.DeserializeObject<SlideshowJsonModel>(apiResponse.Data);
            }
            catch (JsonException)
            {
                return new SlideshowFrame(ApiStatus.Failed(apiType.ToString(), "Exception while deserializing API response", apiResponse.LastUpdateTime));
            }

            if (string.IsNullOrWhiteSpace(slideshowData?.Title)
                || string.IsNullOrWhiteSpace(slideshowData?.Source)
                || string.IsNullOrWhiteSpace(slideshowData?.Url))
            {
                return new SlideshowFrame(ApiStatus.Failed(apiType.ToString(), "Missing required data from api", apiResponse.LastUpdateTime));
            }

            var status = ApiStatus.Success(apiType.ToString(), DateTime.Now, ApiSource.Prod);
            return new SlideshowFrame() 
            {
                Title = title,
				Subtitle = slideshowData.Title ,
                Source = slideshowData.Source,
                Url = slideshowData.Url,
				Status = status
            };
        }
    }
}
