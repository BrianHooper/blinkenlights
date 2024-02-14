namespace Blinkenlights.Models.ViewModels.Slideshow
{
    using Newtonsoft.Json;

    public class SlideshowJsonModel
    {
        [JsonProperty("Title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty("Source", Required = Required.Always)]
        public string Source { get; set; }

        [JsonProperty("Url", Required = Required.Always)]
        public string Url { get; set; }
    }
}