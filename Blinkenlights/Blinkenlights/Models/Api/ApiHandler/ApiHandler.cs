using Blinkenlights.Models.Api.ApiInfoTypes;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Blinkenlights.Models.Api.ApiHandler
{
    public class ApiHandler : IApiHandler
    {
        private const bool CACHE_ENABLED = true;

		private string CachePath { get; init; }

		private string InstanceSecretsCachePath { get; init; }

		private Mutex Mutex { get; init; }

        private IConfiguration Config { get; init; }

        private IWebHostEnvironment WebHostEnvironment { get; init; }

        private Dictionary<ApiSecretType, InstanceSecret> InstanceSecrets { get; init; }


		public ApiHandler(IWebHostEnvironment environment, IConfiguration config)
        {
			this.WebHostEnvironment = environment;
			this.Mutex = new Mutex();
			this.Config = config;

			this.CachePath = Path.Combine(environment.WebRootPath, "DataSources", "ApiCache.json");
			this.InstanceSecretsCachePath = Path.Combine(environment.WebRootPath, "DataSources", "InstanceSecretsApiCache.json");

			try
			{
				var instanceSecretsSerialized = File.ReadAllText(this.InstanceSecretsCachePath);
				this.InstanceSecrets = JsonConvert.DeserializeObject<Dictionary<ApiSecretType, InstanceSecret>>(instanceSecretsSerialized);
			}
			catch (Exception)
			{
				this.InstanceSecrets = new Dictionary<ApiSecretType, InstanceSecret>();
			}
		}

        public bool CheckForInvalidSecrets(out List<string> invalidSecretsOut)
        {
            var invalidSecrets = new List<string>();
            foreach (var secret in Enum.GetValues<ApiSecretType>())
            {
				if (secret != ApiSecretType.Default 
                    && secret.GetSecretSource() == ApiType.Unknown
                    && !TryGetSecret(secret, out _))
                {
                    var key = secret.GetSecretString();
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

        public bool TryGetSecret(ApiSecretType secretType, out string secret)
        {
            // Upload with:
            // dotnet user-secrets set "OpenWeatherMap:ServiceApiKey" "{Secret}"
            var key = secretType.GetSecretString();
            if (secretType == ApiSecretType.Default || string.IsNullOrWhiteSpace(key))
            {
                secret = null;
                return false;
            }

            var secretSource = secretType.GetSecretSource();
            if (secretSource == ApiType.Unknown)
			{
				secret = Config[key];
				if (string.IsNullOrWhiteSpace(secret))
				{
					secret = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
				}
				return !string.IsNullOrWhiteSpace(secret);
			}
            else
            {
				secret = this.GetInstanceSecret(secretType).Result;
				return !string.IsNullOrWhiteSpace(secret);
			}
		}

		public async Task<string> GetInstanceSecret(ApiSecretType secretType)
		{
			if (this.InstanceSecrets.TryGetValue(secretType, out var existingInstanceSecret) && existingInstanceSecret?.IsValid() == true)
			{
				return existingInstanceSecret.Secret;
			}

            var sourceApiType = secretType.GetSecretSource();
            var sourceApiInfo = sourceApiType.Info();
            var apiSecretResponse = await this.GetRemoteData(sourceApiType, sourceApiInfo);
            var updatedInstanceSecret = sourceApiInfo.ResponseToInstanceSecret(apiSecretResponse);

            if (updatedInstanceSecret?.IsValid() == true)
            {
				this.InstanceSecrets[secretType] = updatedInstanceSecret;

                UpdateInstanceSecretsCache();

                return updatedInstanceSecret.Secret;
            }
            else
            {
                return null;
            }
		}

		private void UpdateInstanceSecretsCache()
		{
			var settings = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Include,
				Formatting = Formatting.Indented,
			};
			var instanceSecretsSerialized = JsonConvert.SerializeObject(this.InstanceSecrets, settings);
			File.WriteAllText(this.InstanceSecretsCachePath, instanceSecretsSerialized);
		}

		private bool TryGetCachedValue(ApiType apiType, int? cacheTimeout, out ApiResponse cachedValue)
        {
            Mutex.WaitOne();
            if (!CACHE_ENABLED || cacheTimeout == null || cacheTimeout == 0 || !File.Exists(CachePath))
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

            cachedValue = ApiResponse.Success(apiType, apiHandlerModule.ApiData, ApiSource.Cache, apiHandlerModule.LastUpdateTime);
            Mutex.ReleaseMutex();
            return true;
        }

        public bool TryUpdateCache(ApiResponse response)
        {
            var apiInfo = response?.ApiType.Info();
            Mutex.WaitOne();
            if (response is null || response.ApiType == ApiType.Unknown || response.ApiSource != ApiSource.Prod || string.IsNullOrWhiteSpace(response.Data) || apiInfo == null || apiInfo.ServerType == ApiServerType.Local)
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

            var apiHandlerModule = new ApiCacheItem()
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
            if (!TryBuildString(apiInfo?.Endpoint(), out var relativePath))
			{
				return ApiResponse.Error(apiType, "Invalid request", ApiSource.Prod);
			}

            var pathParts = new string[] { WebHostEnvironment.WebRootPath, relativePath };
            string path = Path.Combine(pathParts);

            if (!File.Exists(path))
			{
				return ApiResponse.Error(apiType, "Local file does not exist", ApiSource.Prod);
			}

            string content = null;
            try
            {
                content = File.ReadAllText(path);
            }
            catch (Exception)
            {
			}

			if (!string.IsNullOrWhiteSpace(content))
			{
				return ApiResponse.Success(apiType, content, ApiSource.Prod, DateTime.Now);
			}
            else
			{
				return ApiResponse.Error(apiType, "Exception while reading local data file", ApiSource.Prod);
			}
		}

        private async Task<ApiResponse> GetRemoteData(ApiType apiType, IApiInfo apiInfo, params string[] queryParameters)
        {
			if (!TryBuildString(apiInfo?.Endpoint(queryParameters), out var endpointUrl))
			{
				return ApiResponse.Error(apiType, "Invalid request", ApiSource.Prod);
			}

            var client = new RestClient(endpointUrl);
			var authenticationSecrets = apiInfo.AuthenticationSecrets();
			if (authenticationSecrets?.Any() == true)
			{
                if (TryBuildString(authenticationSecrets.ElementAtOrDefault(0), out string username) 
                    && TryBuildString(authenticationSecrets.ElementAtOrDefault(1), out string password))
				{
					client.Authenticator = new HttpBasicAuthenticator(username, password);
				}
                else
                {
					return ApiResponse.Error(apiType, $"Failed to get authentication keys", ApiSource.Prod);
				}
			}

			var request = new RestRequest();
            var parameters = apiInfo.Parameters(); 
            if (parameters?.Any() == true)
			{
				foreach (var parameter in parameters)
				{
					request.AddParameter(parameter.Key, parameter.Value);
				}
			}

			var headers = apiInfo.Headers();
            if (headers?.Any() == true)
            {
                foreach (var header in headers)
                {
                    if (TryBuildString(header.Value, out var formattedHeaderValue))
                    {
                        request.AddHeader(header.Key, formattedHeaderValue);
                    }
                    else
					{
						return ApiResponse.Error(apiType, $"Failed to build header: {header.Key}", ApiSource.Prod);
					}
                }
            }


            RestResponse response;
            try
            {
                response = apiInfo.ApiRestType == ApiRestType.Post ? await client.PostAsync(request) : await client.GetAsync(request);
            }
            catch (HttpRequestException ex)
			{
                if (ex?.StatusCode == System.Net.HttpStatusCode.Unauthorized && apiInfo.InstanceSecrets?.Any() == true)
                {
                    DeleteInstanceSecrets(apiInfo.InstanceSecrets);
                }
				return ApiResponse.Error(apiType, $"Api exception: {ex.Message}", ApiSource.Prod);
			}

            if (response == null)
            {
				return ApiResponse.Error(apiType, "Api call returned null", ApiSource.Prod);
			}

            if (response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
			{
				return ApiResponse.Error(apiType, response.StatusDescription, ApiSource.Prod);
			}

			return ApiResponse.Success(apiType, response.Content, ApiSource.Prod, DateTime.Now);
        }

		private void DeleteInstanceSecrets(List<ApiSecretType> instanceSecrets)
		{
			if (instanceSecrets?.Any() == true)
            {
                foreach(var secret in instanceSecrets)
                {
                    this.InstanceSecrets.Remove(secret);
                }
				UpdateInstanceSecretsCache();
			}
		}

		public async Task<ApiResponse> Fetch(ApiType apiType, params string[] queryParameters)
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
                ApiServerType.Remote => await GetRemoteData(apiType, apiInfo, queryParameters),
                _ => null
            };
        }

        private bool TryBuildString(StringSecretsPair stringSecretsPair, out string formattedString)
        {
            if (stringSecretsPair == null)
            {
                formattedString = null;
				return false;
            }

            if (stringSecretsPair.SecretKeys?.Any() != true)
            {
                if (string.IsNullOrWhiteSpace(stringSecretsPair.StringFormat))
				{
					formattedString = null;
					return false;
				}
                else
                {
					formattedString = stringSecretsPair.StringFormat;
                    return true;
                }
            }

            var secretValues = new List<string>();
            foreach(var secretKey in stringSecretsPair.SecretKeys)
            {
                if (TryGetSecret(secretKey, out var secretValue) && !string.IsNullOrWhiteSpace(secretValue))
                {
					secretValues.Add(secretValue);
				}
                else
				{
					formattedString = null;
					return false;
                }
            }

			formattedString = string.Format(stringSecretsPair.StringFormat, secretValues.ToArray());
            return true;
        }
    }
}
