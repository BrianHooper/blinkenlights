using Blinkenlights.Models.Api.ApiInfoTypes;

namespace Blinkenlights.Models.Api.ApiHandler
{
    public class ApiResponse
    {
        public ApiType ApiType { get; set; }

        public string Data { get; set; }

        public ApiSource ApiSource { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public ApiResponse(ApiType apiType, string data, ApiSource source, DateTime lastUpdateTime)
        {
            ApiType = apiType;
            Data = data;
            ApiSource = source;
            LastUpdateTime = lastUpdateTime;
        }
    }
}
