using Newtonsoft.Json;
using RestSharp;

namespace BlinkenLights.Models.ApiCache
{
    public class ApiCache
    {
        private string CachePath { get; init; }

        private Mutex Mutex { get; init; }

        public ApiCache(string cachePath)
        {
            CachePath = cachePath;
            Mutex = new Mutex();
        }

        public bool TryGetCachedValue(string cacheKey, int cacheTimeoutMinutes, out string cachedValue)
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

            if (apiCacheModel?.Modules?.TryGetValue(cacheKey, out var apiCacheModule) != true || apiCacheModule is null)
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

            cachedValue = apiCacheModule.ApiData;
            Mutex.ReleaseMutex();
            return true;
        }

        public bool TryUpdateCache(string apiCacheKey, string apiResponse)
        {
            Mutex.WaitOne();
            if (string.IsNullOrWhiteSpace(apiCacheKey) || string.IsNullOrWhiteSpace(apiResponse))
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
                LastUpdateTime = DateTime.Now,
                ApiData = apiResponse
            };
            apiCacheModel.Modules[apiCacheKey] = apiCacheModule;
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

        public async Task<string> GetAndUpdateApiValue(string cacheKey, int cacheTimeoutMinutes, string endpointUrl, Dictionary<string, string> headers = null)
        {
            if (TryGetCachedValue(cacheKey, cacheTimeoutMinutes, out var apiResponse))
            {
                return apiResponse;
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

            var response = await client.GetAsync(request);
            if (response?.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
            {
                return null;
            }

            TryUpdateCache(cacheKey, response.Content);
            return response.Content;
        }
    }
}
