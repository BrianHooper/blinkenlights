using Blinkenlights.Models.ApiCache;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Utilities;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ApiResult
{
    public enum ApiState
    {
        Unknown = 0,
        Failed = 1,
        Stale = 2,
        Success = 3
    }

    public class ApiResultBase
    {
        public string Status { get; }

        public ApiResultBase(params ApiStatus[] apiStatuses)
        {
            Status = ApiStatusList.Serialize(apiStatuses);
        }
    }

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

    public class ApiStatusList
    {
        public List<ApiStatus> Items { get; }

        private ApiStatusList(List<ApiStatus> items)
        {
            Items = items;
        }

        public static string Serialize(params ApiStatus[] statusItems)
        {
            var apiStatusList = new ApiStatusList(statusItems.ToList());
            return JsonConvert.SerializeObject(apiStatusList);
        }
    }
}
