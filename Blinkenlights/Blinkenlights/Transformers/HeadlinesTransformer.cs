using BlinkenLights.Models.Headlines;
using Newtonsoft.Json;

namespace BlinkenLights.Transformers
{
    public class HeadlinesTransformer
    {
        public static HeadlinesViewModel GetHeadlinesViewModel(string wikipediaResponse, string nytResponse)
        {
            var nytModel = JsonConvert.DeserializeObject<NewYorkTimesModel>(nytResponse);
            var top3us = nytModel?.results?.Where(r => string.Equals(r?.section, "us", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();
            var top3world = nytModel?.results?.Where(r => string.Equals(r?.section, "world", StringComparison.OrdinalIgnoreCase))?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();

            var wikipediaModel = JsonConvert.DeserializeObject<WikipediaModel>(wikipediaResponse);
            var wikipediaInTheNews = wikipediaModel?.InTheNews?.Take(3)?.Select(a => new HeadlinesArticle(a))?.ToList();

            var headlinesViewModel = new HeadlinesViewModel()
            {
                WikipediaInTheNews = wikipediaInTheNews,
                NewYorkTimesFrontPageUs = top3us,
                NewYorkTimesFrontPageWorld = top3world,
            };

            return headlinesViewModel;
        }
    }
}
