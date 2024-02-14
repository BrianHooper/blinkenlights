namespace Blinkenlights.Models.ViewModels.IssTracker
{
    using Newtonsoft.Json;

    public class IssTrackerJsonModel
    {
        [JsonProperty("image_path", Required = Required.Always)]
        public string ImagePath { get; set; }

        [JsonProperty("last_update_time", Required = Required.Always)]
        public string LastUpdateTime { get; set; }

        [JsonProperty("latitude", Required = Required.Always)]
        public float Latitude { get; set; }

        [JsonProperty("longitude", Required = Required.Always)]
        public float Longitude { get; set; }
    }
}