namespace BlinkenLights.Models.ApiCache
{
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
