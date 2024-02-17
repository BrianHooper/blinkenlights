using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Slideshow
{
    public class SlideshowJsonModel
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Source")]
        public string Source { get; set; }

        [JsonPropertyName("Url")]
        public string Url { get; set; }
    }
}