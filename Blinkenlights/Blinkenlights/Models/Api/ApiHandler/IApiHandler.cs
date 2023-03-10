using Blinkenlights.Models.Api.ApiInfoTypes;

namespace Blinkenlights.Models.Api.ApiHandler
{
    public interface IApiHandler
    {
        public bool TryGetSecret(ApiSecretType secretKey, out string secret);

        public bool TryUpdateCache(ApiResponse response);

        public bool CheckForInvalidSecrets(out List<string> invalidSecretsOut);

        public Task<ApiResponse> Fetch(ApiType apiType, params string[] queryParameters);
    }
}
