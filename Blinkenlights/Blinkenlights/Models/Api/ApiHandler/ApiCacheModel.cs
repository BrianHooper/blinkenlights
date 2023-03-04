namespace Blinkenlights.Models.Api.ApiHandler
{
    public enum ApiSource
    {
        Unknown = 0,
        Cache = 1,
        Prod = 2,
        Error = 3
    }

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
