using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using static Google.Apis.Requests.BatchRequest;

namespace Blinkenlights.Models.Api.ApiResult
{
	public class ApiStatus
    {
        public string Name { get; }

        public string Status { get; }

        public string LastUpdate { get; }

        public ApiState State { get; }

        public ApiSource Source { get; }

        public ApiStatus(ApiType apiType, DateTime? lastUpdateTime, ApiSource? source, ApiState state, string statusMessage = null)
        {
            Name = apiType.ToString();
            Status = statusMessage;
            LastUpdate = lastUpdateTime is null ? string.Empty : lastUpdateTime.Value.ToString("hh:mm tt");
            State = state;
            Source = source is null ? ApiSource.Unknown : source.Value;
        }

        public static ApiStatus Success(ApiType apiType, ApiResponse response, string statusMessage = "Success")
        {
            return new ApiStatus(apiType, response?.LastUpdateTime, response?.ApiSource, ApiState.Success, statusMessage);
        }

        public static ApiStatus Failed(ApiType apiType, ApiResponse response, string statusMessage = null)
        {
            return new ApiStatus(apiType, response?.LastUpdateTime, response?.ApiSource, ApiState.Failed, statusMessage);

        }

        public static ApiStatus Success(ApiType apiType, DateTime? lastUpdateTime, ApiSource? source, string statusMessage = "Success")
        {
            return new ApiStatus(apiType, lastUpdateTime, source, ApiState.Success, statusMessage);
        }

        public static ApiStatus Failed(ApiType apiType, DateTime? lastUpdateTime, ApiSource? source, string statusMessage)
        {
            return new ApiStatus(apiType, lastUpdateTime, source, ApiState.Failed, statusMessage);
        }
    }
}
