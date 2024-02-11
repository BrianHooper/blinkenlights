using Newtonsoft.Json;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.Headlines;
using Blinkenlights.Models.ViewModels;
using LiteDbLibrary;
using Blinkenlights.Dataschemas;

namespace Blinkenlights.Transformers
{

    public class HeadlinesTransformer : TransformerBase
	{
		public HeadlinesTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}
        
		public override IModuleViewModel Transform()
		{
            var nytModel = ProcessNytResponse();
			var wikipediaModel = ProcessPageParseApiResponse(ApiType.Wikipedia, "Wikipedia - In the news");
			var ycombinatorModel = ProcessPageParseApiResponse(ApiType.YCombinator, "YCombinator");
			var rocketModel = ProcessPageParseApiResponse(ApiType.RocketLaunches, "Upcoming rocket launches");
			return new HeadlinesViewModel(nytModel.Result, wikipediaModel.Result, ycombinatorModel.Result, rocketModel.Result);
		}

		private static void PostProcessWikipedia(HeadlinesContainer wikipediaModel)
		{
            var articles = wikipediaModel?.Categories?.FirstOrDefault()?.Articles;
			if (wikipediaModel?.Categories?.Any() != true)
            {
                return;
            }

            foreach(var category in wikipediaModel.Categories)
            {
                category.Articles = category?.Articles?.Select(a => ProcessArticle(a))?.ToList();
            }
		}

		private static HeadlinesArticle ProcessArticle(HeadlinesArticle article)
		{
			if (article.Title == null)
            {
                return null;
            }

            var title = article.Title;
            var titleChars = new List<char>();
            var controlCharacters = new Stack<char>();
            foreach(var c in title)
            {
                if (c == '(')
                {
                    controlCharacters.Push(c);
                }
                else if (c == ')')
                {
                    controlCharacters.Pop();
                }
                else if (!controlCharacters.Any())
                {
                    titleChars.Add(c);
                }
            }

            title = string.Join("", titleChars);
			titleChars = new List<char>();
			for (int i = 0; i < title.Length - 1; i++)
            {
                if (!Char.IsWhiteSpace(title[i]) || !Char.IsPunctuation(title[i + 1]))
                {
                    titleChars.Add(title[i]);
                }
            }
			title = string.Join("", titleChars);

			return new HeadlinesArticle(title, article.Url);
		}

		private async Task<HeadlinesContainer> ProcessNytResponse()
        {
			var response = await this.ApiHandler.Fetch(ApiType.NewYorkTimes);
			if (response is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.NewYorkTimes.ToString(), "Api response is null");
                return new HeadlinesContainer(null, errorStatus);
            }

            var nytModel = !string.IsNullOrWhiteSpace(response?.Data) ? JsonConvert.DeserializeObject<NewYorkTimesModel>(response.Data) : default;
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
                var status = ApiStatus.Success(ApiType.NewYorkTimes.ToString(), response.LastUpdateTime, response.ApiSource);
				this.ApiHandler.TryUpdateCache(response);
				return new HeadlinesContainer(categories, status);
            }
            else
            {
                var errorStatus = ApiStatus.Failed(ApiType.NewYorkTimes.ToString(), "No Results", response.LastUpdateTime);
				return new HeadlinesContainer(null, errorStatus);
			}
        }

        public async Task<HeadlinesContainer> ProcessPageParseApiResponse(ApiType apiType, string title)
        {
            var response = await this.ApiHandler.Fetch(apiType);
			if (response is null)
            {
                var errorStatus = ApiStatus.Failed(apiType.ToString(), "Api response is null", response.LastUpdateTime);
				return new HeadlinesContainer(null, errorStatus);
            }

            if (ApiError.IsApiError(response.Data, out var errorMessage))
            {
                var errorStatus = ApiStatus.Failed(apiType.ToString(), errorMessage, response.LastUpdateTime);
				return new HeadlinesContainer(null, errorStatus);
			}

            if (string.IsNullOrWhiteSpace(response?.Data))
            {
				var errorStatus = ApiStatus.Failed(apiType.ToString(), "Response data is null", response.LastUpdateTime);
				return new HeadlinesContainer(null, errorStatus);
			}

            PageParseApiModel model;
            try
            {
                model = JsonConvert.DeserializeObject<PageParseApiModel>(response.Data);
			}
            catch (JsonException)
            {
				var errorStatus = ApiStatus.Failed(apiType.ToString(), "Exception deserializing response", response.LastUpdateTime);
				return new HeadlinesContainer(null, errorStatus);
			}

            var articles = model?.stories?.Select(a => new HeadlinesArticle(a))?.Take(3)?.ToList();
            if (articles?.Any() == true)
            {
                var status = ApiStatus.Success(apiType.ToString(), response.LastUpdateTime, response.ApiSource);
                var categories = new List<HeadlinesCategory>() { new HeadlinesCategory(title, articles) };
				this.ApiHandler.TryUpdateCache(response);
				var headlinesContatiner = new HeadlinesContainer(categories, status);
                
                // TODO Process this on the python api side
                if (apiType == ApiType.Wikipedia)
                {
                    PostProcessWikipedia(headlinesContatiner);
                }
                
                return headlinesContatiner;
            }
            else
            {
                var errorStatus = ApiStatus.Failed(apiType.ToString(), "No headlines created", response.LastUpdateTime);
                return new HeadlinesContainer(null, errorStatus);
            }
        }
	}
}
