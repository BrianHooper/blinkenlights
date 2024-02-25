using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.OuterSpace
{
    public class IssTrackerJsonModel
    {
        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; }

        [JsonPropertyName("last_update_time")]
        public string LastUpdateTime { get; set; }

        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }
    }
}