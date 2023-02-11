using Blinkenlights.Models.ApiCache;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Models.Headlines;
using Newtonsoft.Json;

namespace BlinkenLights.Transformers
{

    public class HeadlinesTransformer
    {
        public const string WikiApiName = "Wikipedia - In The News";
        public const string WikiApiKey = "WikiITN";
        public const string NytApiName = "New York Times";
        public const string NytApiKey = "nyt";

        public static HeadlinesViewModel GetHeadlinesViewModel(ApiResponse wikipediaResponse, ApiResponse nytResponse)
        {
            ProcessNytResponse(nytResponse, out var top3us, out var top3world, out var nytStatus);
            ProcessWikipediaResponse(wikipediaResponse, out var wikipediaInTheNews, out var wikiApiStatus);

            var headlinesViewModel = new HeadlinesViewModel()
            {
                WikipediaInTheNews = wikipediaInTheNews,
                NewYorkTimesFrontPageUs = top3us,
                NewYorkTimesFrontPageWorld = top3world,
                Status = ApiStatusList.Serialize(nytStatus, wikiApiStatus)
            };

            return headlinesViewModel;
        }

        private static void ProcessNytResponse(ApiResponse nytResponse, out List<HeadlinesArticle> top3us, out List<HeadlinesArticle> top3world, out ApiStatus status)
        {
            if (nytResponse is null)
            {
                top3us = null;
                top3world = null;
                status = new ApiStatus(NytApiName, NytApiKey, "nytResponse is null", null, ApiState.Error);
                return;
            }

            var nytModel = !string.IsNullOrWhiteSpace(nytResponse?.Data) ? JsonConvert.DeserializeObject<NewYorkTimesModel>(nytResponse.Data) : default(NewYorkTimesModel);
            top3us = nytModel?.results?.Where(r => string.Equals(r?.section, "us", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();
            top3world = nytModel?.results?.Where(r => string.Equals(r?.section, "world", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();

            if (top3us?.Any() == true || top3world?.Any() == true)
            {
                status = new ApiStatus(NytApiName, NytApiKey, "Good", nytResponse.LastUpdateTime.ToString(), ApiState.Good);
            }
            else
            {
                top3us = null;
                top3world = null;
                status = new ApiStatus(NytApiName, NytApiKey, "Failed - no results", nytResponse.LastUpdateTime.ToString(), ApiState.Error);
            }
        }

        public static void ProcessWikipediaResponse(ApiResponse wikipediaResponse, out List<HeadlinesArticle> headlinesArticles, out ApiStatus status)
        {
            if (wikipediaResponse is null)
            {
                headlinesArticles = null;
                status = new ApiStatus(WikiApiName, WikiApiKey, "Failed", null, ApiState.Error);
                return;
            }

            var wikipediaModel = !string.IsNullOrWhiteSpace(wikipediaResponse?.Data) ? JsonConvert.DeserializeObject<WikipediaModel>(wikipediaResponse.Data) : default(WikipediaModel);
            headlinesArticles = wikipediaModel?.InTheNews?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();

            if (headlinesArticles?.Any() == true)
            {
                status = new ApiStatus(WikiApiName, WikiApiKey, "Good", wikipediaResponse.LastUpdateTime.ToString(), ApiState.Good);
            }
            else
            {
                headlinesArticles = null;
                status = new ApiStatus(WikiApiName, WikiApiKey, "Failed", wikipediaResponse.LastUpdateTime.ToString(), ApiState.Error);
            }
        }
    }
}
