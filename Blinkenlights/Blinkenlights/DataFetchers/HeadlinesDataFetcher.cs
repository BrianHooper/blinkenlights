using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class HeadlinesDataFetcher : DataFetcherBase<HeadlinesData>
    {
        public HeadlinesDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<HeadlinesDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
		{
            Start();
        }

        public override HeadlinesData GetRemoteData(HeadlinesData existingData = null, bool overwrite = false)
        {
            var nytModel = ProcessNytResponse(existingData?.Headlines?.FirstOrDefault(h => string.Equals("NYT", h?.Key)), overwrite);
            var wikipediaModel = ProcessPageParseApiResponse(existingData?.Headlines?.FirstOrDefault(h => string.Equals("WIKI", h?.Key)), "WIKI", ApiType.Wikipedia, "Wikipedia - In the news", overwrite);
            var ycombinatorModel = ProcessPageParseApiResponse(existingData?.Headlines?.FirstOrDefault(h => string.Equals("YCOMB", h?.Key)), "YCOMB", ApiType.YCombinator, "YCombinator", overwrite);
            return new HeadlinesData()
            {
                Headlines = new List<HeadlinesContainer>()
                {
                    nytModel.Result,
                    wikipediaModel.Result,
                    ycombinatorModel.Result,
                },
                TimeStamp = DateTime.Now,
            };
        }

        private static void PostProcessWikipedia(HeadlinesContainer wikipediaModel)
        {
            var articles = wikipediaModel?.Categories?.FirstOrDefault()?.Articles;
            if (wikipediaModel?.Categories?.Any() != true)
            {
                return;
            }

            foreach (var category in wikipediaModel.Categories)
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
            foreach (var c in title)
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

        private async Task<HeadlinesContainer> ProcessNytResponse(HeadlinesContainer existingData, bool overwrite)
        {
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.NewYorkTimes.Info()))
			{
				return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.NewYorkTimes} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.NewYorkTimes);
            if (response is null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.NewYorkTimes, "Api response is null");
                return HeadlinesContainer.Clone(existingData, errorStatus);
            }

            var nytModel = !string.IsNullOrWhiteSpace(response?.Data) ? JsonSerializer.Deserialize<NewYorkTimesModel>(response.Data) : default;
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
                var status = this.ApiStatusFactory.Success(ApiType.NewYorkTimes, response.LastUpdateTime, response.ApiSource);
                return new HeadlinesContainer()
                {
                    Status = status,
                    Key = "NYT",
                    Categories = categories
                };
            }
            else
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.NewYorkTimes, "No Results", response.LastUpdateTime);
				return HeadlinesContainer.Clone(existingData, errorStatus);
			}
        }

        public async Task<HeadlinesContainer> ProcessPageParseApiResponse(HeadlinesContainer existingData, string key, ApiType apiType, string title, bool overwrite)
        {
			if (!overwrite && !IsExpired(existingData?.Status, apiType.Info()))
			{
				return existingData;
			}

			this.Logger.LogInformation($"Calling {apiType} remote API");
			var response = await this.ApiHandler.Fetch(apiType);
            if (response is null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(apiType, "Api response is null", response.LastUpdateTime);
				return HeadlinesContainer.Clone(existingData, errorStatus);
			}

            if (ApiError.IsApiError(response.Data, out var errorMessage))
            {
                var errorStatus = this.ApiStatusFactory.Failed(apiType, errorMessage, response.LastUpdateTime);
				return HeadlinesContainer.Clone(existingData, errorStatus);
			}

            if (string.IsNullOrWhiteSpace(response?.Data))
            {
                var errorStatus = this.ApiStatusFactory.Failed(apiType, "Response data is null", response.LastUpdateTime);
				return HeadlinesContainer.Clone(existingData, errorStatus);
			}

            PageParseApiModel model;
            try
            {
                model = JsonSerializer.Deserialize<PageParseApiModel>(response.Data);
            }
            catch (JsonException)
            {
                var errorStatus = this.ApiStatusFactory.Failed(apiType, "Exception deserializing response", response.LastUpdateTime);
				return HeadlinesContainer.Clone(existingData, errorStatus);
			}

            var articles = model?.stories?.Select(a => new HeadlinesArticle(a))?.Take(3)?.ToList();
            if (articles?.Any() == true)
            {
                var status = this.ApiStatusFactory.Success(apiType, response.LastUpdateTime, response.ApiSource);
                var categories = new List<HeadlinesCategory>() { new HeadlinesCategory(title, articles) };
                var headlinesContatiner = new HeadlinesContainer()
                {
                    Categories = categories,
                    Status = status,
                    Key = key
                };

                // TODO Process this on the python api side
                if (apiType == ApiType.Wikipedia)
                {
                    PostProcessWikipedia(headlinesContatiner);
                }

                return headlinesContatiner;
            }
            else
            {
                var errorStatus = this.ApiStatusFactory.Failed(apiType, "No headlines created", response.LastUpdateTime);
				return HeadlinesContainer.Clone(existingData, errorStatus);
			}
        }
    }
}
