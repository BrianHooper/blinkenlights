using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel;

namespace BlinkenLights.Models.ApiCache
{
    public enum ApiType
    {
        Unknown = 0,
        TimeZone = 1,
        Meh = 2,
        Life360 = 3,
        WWII = 4,
        Wikipedia = 5,
        NewYorkTimes = 6,
        GoogleCalendar = 7,
        VisualCrossingWeather = 8
    }

    public enum ApiSecret
    {
        Default = 0,
        [Description("Life360:AuthorizationToken")]
        Life360AuthorizationToken = 1,
        [Description("Life360:CircleId")]
        Life360CircleId = 2,
        [Description("VisualCrossing:ServiceApiKey")]
        VisualCrossingServiceApiKey = 3,
        [Description("Meh:ServiceApiKey")]
        MehServiceApiKey = 4,
        [Description("NewYorkTimes:ServiceApiKey")]
        NewYorkTimesServiceApiKey = 5,
        [Description("GoogleCalendar:UserAccount")]
        GoogleCalendarUserAccount = 6,
        [Description("GoogleCalendar:ApiServiceKey")]
        GoogleCalendarApiServiceKey = 7,
    }

    static class ApiSecretExtensions
    {
        public static string ToDescriptionString(this ApiSecret val)
        {
            var apiSecretType = val.GetType();
            var vts = val.ToString();
            var field = apiSecretType.GetField(vts);
            var tpof = typeof(DescriptionAttribute);
            var customs = field.GetCustomAttributes(tpof, false);
            var descriptionAttributes = customs as DescriptionAttribute[];

            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    public class ApiCache
    {
        private string CachePath { get; init; }

        private Mutex Mutex { get; init; }

        public ApiCache(string cachePath)
        {
            CachePath = cachePath;
            Mutex = new Mutex();
        }

        public static bool CheckForInvalidSecrets(IConfiguration config, out List<string> invalidSecretsOut)
        {
            var invalidSecrets = new List<string>();
            foreach (var secret in Enum.GetValues<ApiSecret>())
            {
                if (secret != ApiSecret.Default && !TryGetSecret(config, secret, out var secretValue))
                {
                    var key = secret.ToDescriptionString();
                    invalidSecrets.Add(key);
                }
            }

            if (invalidSecrets.Any())
            {
                invalidSecretsOut = invalidSecrets;
                return true;
            }
            else
            {
                invalidSecretsOut = null;
                return false;
            }
        }

        public static bool TryGetSecret(IConfiguration config, ApiSecret secretKey, out string secret)
        {
            // Upload with:
            // dotnet user-secrets set "OpenWeatherMap:ServiceApiKey" "{Secret}"
            var key = secretKey.ToDescriptionString();
            if (secretKey == ApiSecret.Default || string.IsNullOrWhiteSpace(key))
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

        public bool TryGetCachedValue(ApiType apiType, int cacheTimeoutMinutes, out ApiResponse cachedValue)
        {
            Mutex.WaitOne();
            if (cacheTimeoutMinutes == 0 || !File.Exists(CachePath))
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            var stringData = File.ReadAllText(CachePath);
            ApiCacheModel apiCacheModel;
            try
            {
                apiCacheModel = JsonConvert.DeserializeObject<ApiCacheModel>(stringData);
            }
            catch (Exception)
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            if (apiCacheModel?.Modules?.TryGetValue(apiType.ToString(), out var apiCacheModule) != true || apiCacheModule is null)
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            var cacheStalenessMinutes = DateTime.Now.Subtract(apiCacheModule.LastUpdateTime).TotalMinutes;
            if (cacheTimeoutMinutes > 0 && cacheStalenessMinutes >= cacheTimeoutMinutes || string.IsNullOrWhiteSpace(apiCacheModule.ApiData))
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            cachedValue = new ApiResponse(apiType, apiCacheModule.ApiData, ApiSource.Cache, apiCacheModule.LastUpdateTime);
            Mutex.ReleaseMutex();
            return true;
        }

        public bool TryUpdateCache(ApiType apiType, DateTime lastUpdateTime, string apiResponse)
        {
            Mutex.WaitOne();
            if (apiType == ApiType.Unknown || string.IsNullOrWhiteSpace(apiResponse))
            {
                Mutex.ReleaseMutex();
                return false;
            }

            ApiCacheModel apiCacheModel = null;
            if (File.Exists(CachePath))
            {
                var stringData = File.ReadAllText(CachePath);
                try
                {
                    apiCacheModel = JsonConvert.DeserializeObject<ApiCacheModel>(stringData);
                }
                catch (Exception)
                {
                }
            }

            if (apiCacheModel?.Modules == null)
            {
                apiCacheModel = new ApiCacheModel()
                {
                    Modules = new Dictionary<string, ApiCacheModule>()
                };
            }

            ApiCacheModule apiCacheModule = new ApiCacheModule()
            {
                LastUpdateTime = lastUpdateTime,
                ApiData = apiResponse
            };
            apiCacheModel.Modules[apiType.ToString()] = apiCacheModule;
            var serializedCacheModel = JsonConvert.SerializeObject(apiCacheModel, Formatting.Indented);

            try
            {
                File.WriteAllText(CachePath, serializedCacheModel);
                Mutex.ReleaseMutex();
                return true;
            }
            catch (Exception)
            {
                Mutex.ReleaseMutex();
                return false;
            }
        }

        public async Task<ApiResponse> GetAndUpdateApiValue(ApiType apiType, int cacheTimeoutMinutes, string endpointUrl, Dictionary<string, string> headers = null)
        {
            if (TryGetCachedValue(apiType, cacheTimeoutMinutes, out var apiResponseCached))
            {
                return apiResponseCached;
            }

            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                return null;
            }

            var client = new RestClient(endpointUrl);

            var request = new RestRequest();
            if (headers?.Any() == true)
            {
                foreach(var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            var response = default(RestResponse);
            try
            {
                response = await client.GetAsync(request);
            }
            catch (HttpRequestException)
            {
                return null;
            }

            if (response?.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
            {
                return null;
            }

            var lastUpdateTime = DateTime.Now;
            TryUpdateCache(apiType, lastUpdateTime, response.Content);
            return new ApiResponse(apiType, response.Content, ApiSource.Prod, lastUpdateTime);
        }
    }
}
