using Blinkenlights.Models.ApiCache;
using Blinkenlights.Models.ApiResult;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Utilities;
using Newtonsoft.Json;

namespace Blinkenlights.Models
{
    public class GenericApiViewModel: ApiResultBase
    {
        public string ApiData { get; }

        private GenericApiViewModel(string apiData, ApiStatus status) : base(status)
        {
            ApiData = apiData;
        }

        public static string FromApiStatus(string apiData, ApiStatus status)
        {
            var viewModel = new GenericApiViewModel(apiData, status);
            return JsonConvert.SerializeObject(viewModel);
        }
    }
}
