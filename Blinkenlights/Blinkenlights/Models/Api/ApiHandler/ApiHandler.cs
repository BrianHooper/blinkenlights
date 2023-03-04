using Blinkenlights.Models.Api.ApiInfoTypes;
using Newtonsoft.Json;
using RestSharp;

namespace Blinkenlights.Models.Api.ApiHandler
{
    public class ApiHandler : IApiHandler
    {
        private const bool CACHE_ENABLED = true;

        private string CachePath { get; init; }

        private Mutex Mutex { get; init; }

        private IConfiguration Config { get; init; }

        private IWebHostEnvironment WebHostEnvironment { get; init; }

        public ApiHandler(IWebHostEnvironment environment, IConfiguration config)
        {
            WebHostEnvironment = environment;
            CachePath = Path.Combine(environment.WebRootPath, "DataSources", "ApiCache.json");
            Mutex = new Mutex();
            Config = config;
        }

        public bool CheckForInvalidSecrets(out List<string> invalidSecretsOut)
        {
            var invalidSecrets = new List<string>();
            foreach (var secret in Enum.GetValues<ApiSecretType>())
            {
                if (secret != ApiSecretType.Default && !TryGetSecret(secret, out var secretValue))
                {
                    var key = secret.ToSecretString();
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

        public bool TryGetSecret(ApiSecretType secretKey, out string secret)
        {
            // Upload with:
            // dotnet user-secrets set "OpenWeatherMap:ServiceApiKey" "{Secret}"
            var key = secretKey.ToSecretString();
            if (secretKey == ApiSecretType.Default || string.IsNullOrWhiteSpace(key))
            {
                secret = null;
                return false;
            }

            secret = Config[key];
            if (string.IsNullOrWhiteSpace(secret))
            {
                secret = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
            }

            return !string.IsNullOrWhiteSpace(secret);
        }

        private bool TryGetCachedValue(ApiType apiType, int cacheTimeout, out ApiResponse cachedValue)
        {
            Mutex.WaitOne();
            if (!CACHE_ENABLED || cacheTimeout == 0 || !File.Exists(CachePath))
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            var stringData = File.ReadAllText(CachePath);
            ApiCacheModel apiHandlerModel;
            try
            {
                apiHandlerModel = JsonConvert.DeserializeObject<ApiCacheModel>(stringData);
            }
            catch (Exception)
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            if (apiHandlerModel?.Items?.TryGetValue(apiType.ToString(), out var apiHandlerModule) != true || apiHandlerModule is null)
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            var cacheStalenessMinutes = DateTime.Now.Subtract(apiHandlerModule.LastUpdateTime).TotalMinutes;
            if (cacheTimeout > 0 && cacheStalenessMinutes >= cacheTimeout || string.IsNullOrWhiteSpace(apiHandlerModule.ApiData))
            {
                cachedValue = null;
                Mutex.ReleaseMutex();
                return false;
            }

            cachedValue = new ApiResponse(apiType, apiHandlerModule.ApiData, ApiSource.Cache, apiHandlerModule.LastUpdateTime);
            Mutex.ReleaseMutex();
            return true;
        }

        public bool TryUpdateCache(ApiResponse response)
        {
            Mutex.WaitOne();
            if (response is null || response.ApiType == ApiType.Unknown || response.ApiSource != ApiSource.Prod || string.IsNullOrWhiteSpace(response.Data))
            {
                Mutex.ReleaseMutex();
                return false;
            }

            ApiCacheModel apiHandlerModel = null;
            if (File.Exists(CachePath))
            {
                var stringData = File.ReadAllText(CachePath);
                try
                {
                    apiHandlerModel = JsonConvert.DeserializeObject<ApiCacheModel>(stringData);
                }
                catch (Exception)
                {
                }
            }

            if (apiHandlerModel?.Items == null)
            {
                apiHandlerModel = new ApiCacheModel()
                {
                    Items = new Dictionary<string, ApiCacheItem>()
                };
            }

            ApiCacheItem apiHandlerModule = new ApiCacheItem()
            {
                LastUpdateTime = response.LastUpdateTime,
                ApiData = response.Data
            };
            apiHandlerModel.Items[response.ApiType.ToString()] = apiHandlerModule;
            var serializedCacheModel = JsonConvert.SerializeObject(apiHandlerModel, Formatting.Indented);

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

        private ApiResponse GetLocalData(ApiType apiType, IApiInfo apiInfo)
        {
            var relativePath = this.BuildString(apiInfo?.Endpoint());
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            var pathParts = new string[] { WebHostEnvironment.WebRootPath, relativePath };
            string path = Path.Combine(pathParts);

            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var content = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return new ApiResponse(apiType, content, ApiSource.Prod, DateTime.Now);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        private async Task<ApiResponse> GetRemoteData(ApiType apiType, IApiInfo apiInfo)
        {
            var endpointFormat = apiInfo?.Endpoint();
            var endpointUrl = this.BuildString(endpointFormat);
            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                return null;
            }

            var client = new RestClient(endpointUrl);

            var request = new RestRequest();
            var headers = apiInfo.Headers();
            if (headers?.Any() == true)
            {
                foreach (var header in headers)
                {
                    var headerValue = BuildString(header.Value);

                    // TODO catch & report if secret fetch fails
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        request.AddHeader(header.Key, headerValue);
                    }
                }
            }

            RestResponse response;
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

            var apiResponse = new ApiResponse(apiType, response.Content, ApiSource.Prod, DateTime.Now);
            return apiResponse;
        }

        public async Task<ApiResponse> Fetch(ApiType apiType)
        {
            var apiInfo = apiType.Info();
            if (apiInfo is null)
            {
                return null;
            }

            var cacheTimeout = apiInfo.CacheTimeout;
            if (TryGetCachedValue(apiType, cacheTimeout, out var apiResponseCached))
            {
                return apiResponseCached;
            }

            return apiInfo.ServerType switch
            {
                ApiServerType.Local => GetLocalData(apiType, apiInfo),
                ApiServerType.Remote => await GetRemoteData(apiType, apiInfo),
                _ => null
            };
        }

        private string GetSecret(ApiSecretType key)
        {
            if (TryGetSecret(key, out var secret))
            {
                return secret;
            }
            else
            {
                return null;
            }
        }

        private string BuildString(StringSecretsPair stringSecretsPair)
        {
            if (stringSecretsPair == null)
            {
                return null;
            }

            if (stringSecretsPair.SecretKeys?.Any() != true)
            {
                if (string.IsNullOrWhiteSpace(stringSecretsPair.StringFormat))
                {
                    return null;
                }
                else
                {
                    return stringSecretsPair.StringFormat;
                }
            }

            // TODO Return log message when secret fetch fails
            var secretValues = stringSecretsPair.SecretKeys.Select(s => GetSecret(s))?.ToArray();
            if (secretValues?.Any(s => string.IsNullOrWhiteSpace(s)) == true)
            {
                return null;
            }

            // TODO Catch if number of secrets doesn't match the format
            return string.Format(stringSecretsPair.StringFormat, secretValues);
        }
    }
}
