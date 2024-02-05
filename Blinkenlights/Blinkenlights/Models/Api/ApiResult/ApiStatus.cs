using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using System.Text;

namespace Blinkenlights.Models.Api.ApiResult
{
	public class ApiStatus
    {
        public string Name { get; init; }

        public string Status { get; init; }

        public string LastUpdate { get; init; }

        public ApiState State { get; init; }

        public ApiSource Source { get; init; }

		private ApiStatus(ApiType apiType, DateTime? lastUpdateTime, ApiSource? source, ApiState state, string statusMessage)
        {
            Name = apiType.ToString();
            LastUpdate = lastUpdateTime is null || state == ApiState.Failed ? string.Empty : lastUpdateTime.Value.ToString("hh:mm tt");
            State = state;
            Source = source is null ? ApiSource.Unknown : source.Value;

			Status = BuildStatusString(statusMessage);
		}

        private string BuildStatusString(string statusMessage)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{Name}] ");
			sb.Append($"State: {State}, ");
			sb.Append($"Source: {Source}, ");
			sb.Append($"LastUpdate: {LastUpdate}, ");
			sb.Append($"StatusMessage: {statusMessage}, ");

			return sb.ToString();
        }

        public static ApiStatus Success(ApiType apiType, ApiResponse response, string statusMessage = "Success")
        {
            return new ApiStatus(apiType, response?.LastUpdateTime, response?.ApiSource, ApiState.Success, statusMessage);
        }

        public static ApiStatus Success(ApiType apiType, DateTime? lastUpdateTime, ApiSource? source, string statusMessage = "Success")
        {
            return new ApiStatus(apiType, lastUpdateTime, source, ApiState.Success, statusMessage);
        }

        public static ApiStatus Failed(ApiType apiType, string statusMessage, DateTime? lastUpdateTime = null)
        {
            return new ApiStatus(apiType, lastUpdateTime, ApiSource.Error, ApiState.Failed, statusMessage);
        }
    }
}
