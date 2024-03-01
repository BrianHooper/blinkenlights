using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.Slideshow;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class SlideshowDataFetcher : DataFetcherBase<SlideshowData>
    {
        public SlideshowDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<SlideshowDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

		protected override SlideshowData GetRemoteData(SlideshowData existingData = null, bool overwrite = false)
		{
			var apod = GetPageParsePicOfTheDay(existingData, ApiType.Astronomy, "NASA", "NASA Picture of the Day", overwrite);
			var wiki = GetPageParsePicOfTheDay(existingData, ApiType.WikiPotd, "WIKI", "Wikipedia Picture of the Day", overwrite);

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

        public async Task<SlideshowFrame> GetPageParsePicOfTheDay(SlideshowData existingData, ApiType apiType, string key, string title, bool overwrite)
        {
            var existingFrameData = existingData?.Frames?.FirstOrDefault(f => string.Equals(key, f?.Key));
            if (!overwrite && !IsExpired(existingFrameData?.Status, apiType.Info()))
			{
				return existingFrameData;
            }

			this.Logger.LogInformation($"Calling {apiType} remote API");
			var apiResponse = await this.ApiHandler.Fetch(apiType);
            if (apiResponse is null)
            {
                return new SlideshowFrame(this.ApiStatusFactory.Failed(apiType, "API Response is null"));
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                return new SlideshowFrame(this.ApiStatusFactory.Failed(apiType, "API Response data is empty", apiResponse.LastUpdateTime));
            }

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
            {
                return new SlideshowFrame(this.ApiStatusFactory.Failed(apiType, errorMessage, apiResponse.LastUpdateTime));
            }

            SlideshowJsonModel slideshowData;
            try
            {
                slideshowData = JsonSerializer.Deserialize<SlideshowJsonModel>(apiResponse.Data);
            }
            catch (JsonException)
            {
                return new SlideshowFrame(this.ApiStatusFactory.Failed(apiType, "Exception while deserializing API response", apiResponse.LastUpdateTime));
            }

            if (string.IsNullOrWhiteSpace(slideshowData?.Title)
                || string.IsNullOrWhiteSpace(slideshowData?.Source)
                || string.IsNullOrWhiteSpace(slideshowData?.Url))
            {
                return new SlideshowFrame(this.ApiStatusFactory.Failed(apiType, "Missing required data from api", apiResponse.LastUpdateTime));
            }

            var status = this.ApiStatusFactory.Success(apiType, DateTime.Now, ApiSource.Prod);
            return new SlideshowFrame() 
            {
                Title = title,
				Subtitle = slideshowData.Title ,
                Source = slideshowData.Source,
                Url = slideshowData.Url,
				Status = status,
                Key = key
            };
        }
    }
}
