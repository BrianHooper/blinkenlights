using Blinkenlights.Models.ApiCache;
using Blinkenlights.Models.ApiResult;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Models.Headlines;
using Newtonsoft.Json;

namespace BlinkenLights.Transformers
{

    public class HeadlinesTransformer
    {
        public const ApiType WikiApiType = ApiType.Wikipedia;
        public const ApiType NytApiType = ApiType.NewYorkTimes;

        public static HeadlinesViewModel GetHeadlinesViewModel(ApiResponse wikipediaResponse, ApiResponse nytResponse)
        {
            ProcessNytResponse(nytResponse, out var top3us, out var top3world, out var nytStatus);
            ProcessWikipediaResponse(wikipediaResponse, out var wikipediaInTheNews, out var wikiApiStatus);

            var headlinesViewModel = new HeadlinesViewModel(wikipediaInTheNews, wikiApiStatus, top3us, top3world, nytStatus);
            return headlinesViewModel;
        }

        private static void ProcessNytResponse(ApiResponse nytResponse, out List<HeadlinesArticle> top3us, out List<HeadlinesArticle> top3world, out ApiStatus status)
        {
            if (nytResponse is null)
            {
                top3us = null;
                top3world = null;
                status = ApiStatus.Failed(NytApiType, null, "Response is null");
                return;
            }

            var nytModel = !string.IsNullOrWhiteSpace(nytResponse?.Data) ? JsonConvert.DeserializeObject<NewYorkTimesModel>(nytResponse.Data) : default(NewYorkTimesModel);
            top3us = nytModel?.results?.Where(r => string.Equals(r?.section, "us", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();
            top3world = nytModel?.results?.Where(r => string.Equals(r?.section, "world", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();

            if (top3us?.Any() == true || top3world?.Any() == true)
            {
                status = ApiStatus.Success(NytApiType, nytResponse);
            }
            else
            {
                top3us = null;
                top3world = null;
                status = ApiStatus.Failed(NytApiType, null, "No results");
            }
        }

        public static void ProcessWikipediaResponse(ApiResponse wikipediaResponse, out List<HeadlinesArticle> headlinesArticles, out ApiStatus status)
        {
            if (wikipediaResponse is null)
            {
                headlinesArticles = null;
                status = ApiStatus.Failed(WikiApiType, null, "Response is null");
                return;
            }

            var wikipediaModel = !string.IsNullOrWhiteSpace(wikipediaResponse?.Data) ? JsonConvert.DeserializeObject<WikipediaModel>(wikipediaResponse.Data) : default(WikipediaModel);
            headlinesArticles = wikipediaModel?.InTheNews?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();

            if (headlinesArticles?.Any() == true)
            {
                status = ApiStatus.Success(WikiApiType, wikipediaResponse);
            }
            else
            {
                headlinesArticles = null;
                status = ApiStatus.Failed(WikiApiType, wikipediaResponse, "No headlines created");
            }
        }
    }
}
