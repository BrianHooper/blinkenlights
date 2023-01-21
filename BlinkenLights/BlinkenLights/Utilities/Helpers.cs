using BlinkenLights.Models.ApiCache;
using Newtonsoft.Json;
using RestSharp;

namespace BlinkenLights.Utilities
{
    public class Helpers
    {
        public static DateTime FromEpoch(long epoch, bool useUtc = false, bool addOffset = false)
        {
            var dtKind = useUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, dtKind).AddSeconds(epoch);

            if (addOffset)
            {
                dt.Add(DateTimeOffset.Now.Offset);
            }
            return dt;
        }

        public static bool TryGetSecret(IConfiguration config, string key, out string secret)
        {
            // Upload with:
            // dotnet user-secrets set "OpenWeatherMap:ServiceApiKey" "{Secret}"
            if (string.IsNullOrWhiteSpace(key))
            {
                secret = null;
                return false;
            }

            secret = config[key];
            if (string.IsNullOrWhiteSpace(secret))
            {
                secret = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
            }

            return !string.IsNullOrWhiteSpace(secret);
        }

        public static async Task<string> CallAndUpdateApi(string endpointUrl, ApiCache apiCache, string apiCacheKey)
        {
            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                return null;
            }

            var client = new RestClient(endpointUrl);

            var request = new RestRequest();
            var response = await client.GetAsync(request);
            if (response?.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
            {
                return null;
            }

            apiCache.TryUpdateCache(apiCacheKey, response.Content);
            return response.Content;
        }

        public static string ApiError(string errorMessage)
        {
            return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", errorMessage } });
        }
    }
}
