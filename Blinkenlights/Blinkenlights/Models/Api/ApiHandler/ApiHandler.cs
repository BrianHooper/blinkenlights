using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiInfoTypes;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json;

namespace Blinkenlights.Models.Api.ApiHandler
{
    public class ApiHandler : IApiHandler
    {

        private string InstanceSecretsCachePath { get; init; }

        private Mutex Mutex { get; init; }

        private IConfiguration Config { get; init; }

        private IWebHostEnvironment WebHostEnvironment { get; init; }

        private Dictionary<ApiSecretType, InstanceSecret> InstanceSecrets { get; init; }

        private IDatabaseHandler DatabaseHandler { get; init; }

        private ILogger<ApiHandler> Logger { get; init; }


        public ApiHandler(IWebHostEnvironment environment, IConfiguration config, IDatabaseHandler databaseHandler, ILogger<ApiHandler> logger)
        {
            this.WebHostEnvironment = environment;
            this.DatabaseHandler = databaseHandler;
            this.Logger = logger;
            this.Mutex = new Mutex();
            this.Config = config;

            this.InstanceSecretsCachePath = Path.Combine(environment.WebRootPath, "DataSources", "InstanceSecretsApiCache.json");

            try
            {
                var instanceSecretsSerialized = File.ReadAllText(this.InstanceSecretsCachePath);
                this.InstanceSecrets = JsonSerializer.Deserialize<Dictionary<ApiSecretType, InstanceSecret>>(instanceSecretsSerialized);
            }
            catch (Exception)
            {
                this.InstanceSecrets = new Dictionary<ApiSecretType, InstanceSecret>();
            }
            Logger = logger;
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
            var instanceSecretsSerialized = JsonSerializer.Serialize(this.InstanceSecrets);
            File.WriteAllText(this.InstanceSecretsCachePath, instanceSecretsSerialized);
        }

        private ApiResponse GetLocalData(ApiType apiType, IApiInfo apiInfo)
        {
            if (!TryBuildString(apiInfo?.Endpoint(), out var relativePath))
            {
                return ApiResponse.Error(this.Logger, apiType, "Invalid request", ApiSource.Prod);
            }

            var pathParts = new string[] { WebHostEnvironment.WebRootPath, relativePath };
            string path = Path.Combine(pathParts);

            if (!File.Exists(path))
            {
                return ApiResponse.Error(this.Logger, apiType, "Local file does not exist", ApiSource.Prod);
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
                return ApiResponse.Success(this.Logger, apiType, content, ApiSource.Prod, DateTime.Now);
            }
            else
            {
                return ApiResponse.Error(this.Logger, apiType, "Exception while reading local data file", ApiSource.Prod);
            }
        }

        private async Task<ApiResponse> GetRemoteData(ApiType apiType, IApiInfo apiInfo, string body = null, params string[] queryParameters)
        {
            if (!TryBuildString(apiInfo?.Endpoint(queryParameters), out var endpointUrl))
            {
                return ApiResponse.Error(this.Logger, apiType, "Invalid request", ApiSource.Prod);
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
                    return ApiResponse.Error(this.Logger, apiType, $"Failed to get authentication keys", ApiSource.Prod);
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
                        return ApiResponse.Error(this.Logger, apiType, $"Failed to build header: {header.Key}", ApiSource.Prod);
                    }
                }
            }

            if (body != null)
            {
                request.AddBody(obj: body, contentType: "application/json");
            }

            if (!HandleRateLimit(apiType, apiInfo, out var remainingApiCalls))
            {
                return ApiResponse.Error(this.Logger, apiType, $"{apiType} is rate-limited.", ApiSource.Prod);
            }

            this.Logger.LogInformation($"Calling remote API...: {apiType}, remaninng API calls: {remainingApiCalls}");
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
                return ApiResponse.Error(this.Logger, apiType, $"Api exception: {ex.Message}", ApiSource.Prod, ex.StatusCode.ToString());
            }

            if (response == null)
            {
                return ApiResponse.Error(this.Logger, apiType, "Api call returned null", ApiSource.Prod);
            }

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response?.Content))
            {
                return ApiResponse.Error(this.Logger, apiType, response.StatusDescription, ApiSource.Prod);
            }

            return ApiResponse.Success(this.Logger, apiType, response.Content, ApiSource.Prod, DateTime.Now);
        }

        private bool HandleRateLimit(ApiType apiType, IApiInfo apiInfo, out int remainingApiCalls)
        {
            var apiRateLimitData = this.DatabaseHandler.Get<ApiRateLimit>();
            if (apiRateLimitData?.ApiCalls == null) 
            {
                var apiCalls = new List<DateTime>() { DateTime.Now };
                apiRateLimitData = new ApiRateLimit();
                apiRateLimitData.ApiCalls.Add(apiType.ToString(), apiCalls);
                this.DatabaseHandler.Set(apiRateLimitData);
                remainingApiCalls = (apiInfo.DailyRateLimit ?? 0) - 1;
                return true;
            }

            if (!apiRateLimitData.ApiCalls.TryGetValue(apiType.ToString(), out var previousApiCallCount)) 
            {
                var apiCalls = new List<DateTime>() { DateTime.Now };
                apiRateLimitData.ApiCalls.Add(apiType.ToString(), apiCalls);
                this.DatabaseHandler.Set(apiRateLimitData);
                remainingApiCalls = (apiInfo.DailyRateLimit ?? 0) - 1;
                return true;
            }

			
			var validPrevApiCalls = previousApiCallCount.Where(dt => (DateTime.Now - dt).TotalHours < 24).ToList();
            apiRateLimitData.ApiCalls[apiType.ToString()] = validPrevApiCalls;

			if (validPrevApiCalls.Count() < apiInfo.DailyRateLimit)
            {
				validPrevApiCalls.Add(DateTime.Now);
                this.DatabaseHandler.Set(apiRateLimitData);
                remainingApiCalls = (apiInfo.DailyRateLimit ?? 0) - validPrevApiCalls.Count;
                return true;
            }

			this.DatabaseHandler.Set(apiRateLimitData);
			remainingApiCalls = 0;
            return false;
        }

        private void DeleteInstanceSecrets(List<ApiSecretType> instanceSecrets)
        {
            if (instanceSecrets?.Any() == true)
            {
                foreach (var secret in instanceSecrets)
                {
                    this.InstanceSecrets.Remove(secret);
                }
                UpdateInstanceSecretsCache();
            }
        }

        public async Task<ApiResponse> Fetch(ApiType apiType, string body = null, params string[] queryParameters)
        {
            var apiInfo = apiType.Info();
            if (apiInfo is null)
            {
                return null;
            }

            return apiInfo.ServerType switch
            {
                ApiServerType.Local => GetLocalData(apiType, apiInfo),
                ApiServerType.Remote => await GetRemoteData(apiType, apiInfo, body, queryParameters),
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
            foreach (var secretKey in stringSecretsPair.SecretKeys)
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
