using BlinkenLights.Utilities;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ApiCache
{
    public static class ApiStatusUtil
    {
        public static string GetInvariant(this (string Key, string Value)[] pairs, string key)
        {
            return pairs?.FirstOrDefault(a => a.Key?.Equals("name", StringComparison.OrdinalIgnoreCase) == true).Value ?? string.Empty;
        }
    }

    public enum ApiState
    {
        Unknown = 0,
        Error = 1,
        Stale = 2,
        Good = 3
    }

    //TODO Add "Source" field
    public class ApiStatus
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string Status { get; set; }

        public string LastUpdate { get; set; }

        public ApiState State { get; set; }

        public ApiStatus(string name, string key, string status, string lastUpdate, ApiState state)
        {
            Name = name ?? string.Empty;
            Status = status ?? string.Empty;
            LastUpdate = lastUpdate ?? string.Empty;
            State = state;
            Key = key ?? string.Empty;
        }

        public static string Serialize(string name, string key, string status, string lastUpdate, ApiState state)
        {
            return ApiStatusList.Serialize(new ApiStatus(name, key, status, lastUpdate, state));
        }
    }

    public class ApiStatusList
    {
        public List<ApiStatus> Items { get; set; }

        private ApiStatusList(List<ApiStatus> items)
        {
            Items = items;
        }

        public static string Serialize(params ApiStatus[] statusItems)
        {
            var apiStatusList = new ApiStatusList(statusItems.ToList());
            return Helpers.Serialize(apiStatusList);
        }
    }
}
