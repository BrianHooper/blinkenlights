namespace BlinkenLights.Models.ApiCache
{
    public enum ApiSource
    {
        Unknown = 0,
        Cache = 1,
        Prod = 2,
        Error = 3
    }

    public class ApiResponse
    {
        public string Data { get; set; }

        public ApiSource ApiSource { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public ApiResponse(string data, ApiSource source, DateTime lastUpdateTime)
        {
            this.Data = data;
            this.ApiSource = source;
            this.LastUpdateTime = lastUpdateTime;
        }
    }

    public class ApiCacheModel
    {
        public Dictionary<string, ApiCacheModule> Modules { get; set; }
    }

    public class ApiCacheModule
    {
        public DateTime LastUpdateTime { get; set; }
        public string ApiData { get; set; }
    }
}
