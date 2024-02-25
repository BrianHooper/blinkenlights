using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiInfoTypes;

namespace Blinkenlights.Models.Api.ApiHandler
{
    public enum ApiResultStatus
    {
        Success = 0,
        Error = 1
    }

    public class ApiResponse
    {
        public ApiType ApiType { get; set; }

        public string Data { get; set; }

        public ApiSource ApiSource { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public ApiResultStatus ResultStatus { get; set; }

        public string StatusMessage { get; set; }

        private ApiResponse()
        {

        }

        public static ApiResponse Success(ApiType apiType, string data, ApiSource source, DateTime lastUpdateTime)
        {
            return new ApiResponse()
            {
                ApiType = apiType,
                Data = data,
                ApiSource = source,
                LastUpdateTime = lastUpdateTime,
                ResultStatus = ApiResultStatus.Success
            };
        }

        public static ApiResponse Error(ILogger logger, ApiType apiType, string statusMessage, ApiSource source)
        {
            logger.LogError($"Api error {apiType}: {statusMessage}");
            return new ApiResponse()
            {
                ApiType = apiType,
                StatusMessage = statusMessage,
                ApiSource = source,
                ResultStatus = ApiResultStatus.Error,
            };
        }
    }
}
