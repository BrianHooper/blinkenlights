using Newtonsoft.Json;

namespace BlinkenLights.Models
{
    public class ApiCache
    {
        private string CachePath { get; init; }

        private Mutex Mutex { get; init; }

        public ApiCache(string cachePath)
        {
            this.CachePath = cachePath;
            this.Mutex = new Mutex();
        }

        public bool TryGetCachedValue(string cacheKey, int cacheTimeoutMinutes, out string cachedValue)
        {
            this.Mutex.WaitOne();
            if (cacheTimeoutMinutes == 0 || !File.Exists(CachePath))
            {
                cachedValue = null;
                this.Mutex.ReleaseMutex();
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
                this.Mutex.ReleaseMutex();
                return false;
            }

            if (apiCacheModel?.Modules?.TryGetValue(cacheKey, out var apiCacheModule) != true || apiCacheModule is null)
            {
                cachedValue = null;
                this.Mutex.ReleaseMutex();
                return false;
            }

            var cacheStalenessMinutes = DateTime.Now.Subtract(apiCacheModule.LastUpdateTime).TotalMinutes;
            if ((cacheTimeoutMinutes > 0 && cacheStalenessMinutes >= cacheTimeoutMinutes) || string.IsNullOrWhiteSpace(apiCacheModule.ApiData))
            {
                cachedValue = null;
                this.Mutex.ReleaseMutex();
                return false;
            }

            cachedValue = apiCacheModule.ApiData;
            this.Mutex.ReleaseMutex();
            return true;
        }

        public bool TryUpdateCache(string apiCacheKey, string apiResponse)
        {
            this.Mutex.WaitOne();
            if (string.IsNullOrWhiteSpace(apiCacheKey) || string.IsNullOrWhiteSpace(apiResponse))
            {
                this.Mutex.ReleaseMutex();
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
                this.Mutex.ReleaseMutex();
                return true;
            }
            catch (Exception)
            {
                this.Mutex.ReleaseMutex();
                return false;
            }
        }
    }
}
