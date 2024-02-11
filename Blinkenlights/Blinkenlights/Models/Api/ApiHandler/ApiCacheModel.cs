namespace Blinkenlights.Models.Api.ApiHandler
{
    public class ApiCacheModel
    {
        public Dictionary<string, ApiCacheItem> Items { get; set; }
    }

    public class ApiCacheItem
    {
        public DateTime LastUpdateTime { get; set; }
        public string ApiData { get; set; }
    }
}
