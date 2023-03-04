using Blinkenlights.Transformers;
using Newtonsoft.Json;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.Headlines;
using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Transformers
{

    public class HeadlinesTransformer : TransformerBase
	{
		public HeadlinesTransformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public async override Task<IModuleViewModel> Transform()
		{
            var wikipediaData = await this.ApiHandler.Fetch(ApiType.Wikipedia);
			var nytData = await this.ApiHandler.Fetch(ApiType.NewYorkTimes);
			var ycombinatorData = await this.ApiHandler.Fetch(ApiType.YCombinator);
			var rocketData = await this.ApiHandler.Fetch(ApiType.RocketLaunches);

			ProcessNytResponse(nytData, out var nytModel);
			ProcessPageParseApiResponse(wikipediaData, ApiType.Wikipedia, "Wikipedia - In the news", out var wikipediaModel);
			ProcessPageParseApiResponse(ycombinatorData, ApiType.YCombinator, "YCombinator", out var ycombinatorModel);
			ProcessPageParseApiResponse(rocketData, ApiType.RocketLaunches, "Upcoming rocket launches", out var rocketModel);

			var headlinesViewModel = new HeadlinesViewModel(nytModel, wikipediaModel, ycombinatorModel, rocketModel);
			return headlinesViewModel;
		}

		private void ProcessNytResponse(ApiResponse nytResponse, out HeadlinesContainer headlines)
        {
            if (nytResponse is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.NewYorkTimes, null, "Api response is null");
                headlines = new HeadlinesContainer(null, errorStatus);
                return;
            }

            var nytModel = !string.IsNullOrWhiteSpace(nytResponse?.Data) ? JsonConvert.DeserializeObject<NewYorkTimesModel>(nytResponse.Data) : default(NewYorkTimesModel);
            var top3usItems = nytModel?.results?.Where(r => string.Equals(r?.section, "us", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();
            var top3worldItems = nytModel?.results?.Where(r => string.Equals(r?.section, "world", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();

            var categories = new List<HeadlinesCategory>();
            if (top3usItems?.Any() == true)
            {
                categories.Add(new HeadlinesCategory("NYT - Top US", top3usItems));
            }

            if (top3worldItems?.Any() == true)
            {
                categories.Add(new HeadlinesCategory("NYT - Top World", top3worldItems));
            }

            if (categories.Any())
            {
                var status = ApiStatus.Success(ApiType.NewYorkTimes, nytResponse);
                headlines = new HeadlinesContainer(categories, status);
                this.ApiHandler.TryUpdateCache(nytResponse);
                return;
            }
            else
            {
                var errorStatus = ApiStatus.Failed(ApiType.NewYorkTimes, null, "No Results");
                headlines = new HeadlinesContainer(null, errorStatus);
                return;
            }
        }

        public void ProcessPageParseApiResponse(ApiResponse response, ApiType apiType, string title, out HeadlinesContainer headlines)
        {
            if (response is null)
            {
                var errorStatus = ApiStatus.Failed(apiType, null, "Api response is null");
                headlines = new HeadlinesContainer(null, errorStatus);
                return;
            }

            if (ApiError.IsApiError(response.Data, out var errorMessage))
            {
                var errorStatus = ApiStatus.Failed(apiType, response, errorMessage);
                headlines = new HeadlinesContainer(null, errorStatus);
                return;
            }

            if (string.IsNullOrWhiteSpace(response?.Data))
            {
				var errorStatus = ApiStatus.Failed(apiType, response, "Response data is null");
				headlines = new HeadlinesContainer(null, errorStatus);
				return;
			}

            PageParseApiModel model;
            try
            {
                model = JsonConvert.DeserializeObject<PageParseApiModel>(response.Data);
			}
            catch (JsonException)
            {
				var errorStatus = ApiStatus.Failed(apiType, response, "Exception deserializing response");
				headlines = new HeadlinesContainer(null, errorStatus);
				return;
			}

            var articles = model?.stories?.Select(a => new HeadlinesArticle(a))?.Take(3)?.ToList();
            if (articles?.Any() == true)
            {
                var status = ApiStatus.Success(apiType, response);
                var categories = new List<HeadlinesCategory>() { new HeadlinesCategory(title, articles) };
                headlines = new HeadlinesContainer(categories, status);
				this.ApiHandler.TryUpdateCache(response);
				return;
            }
            else
            {
                var errorStatus = ApiStatus.Failed(apiType, response, "No headlines created");
                headlines = new HeadlinesContainer(null, errorStatus);
                return;
            }
        }
	}
}
