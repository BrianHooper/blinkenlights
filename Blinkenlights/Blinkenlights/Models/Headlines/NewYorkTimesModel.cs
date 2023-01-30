using Newtonsoft.Json;

namespace BlinkenLights.Models.Headlines
{
    public class NewYorkTimesModel
    {
        public List<NewYorkTimesResultsModel> results { get; set; }
    }

    public class NewYorkTimesResultsModel
    {
        public string section { get; set; }
        public string subsection { get; set; }
        public string title { get; set; }

        [JsonProperty("abstract")]
        public string article_abstract { get; set; }
        public string url { get; set; }
    } 
}
