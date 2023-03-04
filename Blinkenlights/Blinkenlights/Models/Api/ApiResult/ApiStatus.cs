using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;

namespace Blinkenlights.Models.Api.ApiResult
{
	public class ApiStatus
    {
        public string Name { get; }

        public string Status { get; }

        public string LastUpdate { get; }

        public ApiState State { get; }

        public ApiSource Source { get; }

        private ApiStatus(ApiType apiType, ApiResponse response, ApiState state, string statusMessage = null)
        {
            Name = apiType.ToString();
            Status = statusMessage;
            LastUpdate = response is null ? string.Empty : response.LastUpdateTime.ToString();
            State = state;
            Source = response is null ? ApiSource.Unknown : response.ApiSource;
        }

        public static ApiStatus Success(ApiType apiType, ApiResponse response, string statusMessage = "Success")
        {
            return new ApiStatus(apiType, response, ApiState.Success, statusMessage);
        }

        public static ApiStatus Failed(ApiType apiType, ApiResponse response, string statusMessage = null)
        {
            return new ApiStatus(apiType, response, ApiState.Failed, statusMessage);
        }

        public static ApiStatus Stale(ApiType apiType, ApiResponse response, string statusMessage = null)
        {
            return new ApiStatus(apiType, response, ApiState.Stale, statusMessage);
        }
    }
}
