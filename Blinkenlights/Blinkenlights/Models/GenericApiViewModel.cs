using Blinkenlights.Models.ApiCache;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Utilities;
using Newtonsoft.Json;

namespace Blinkenlights.Models
{
    public class GenericApiViewModel
    {
        private GenericApiViewModel(string apiData, string status)
        {
            ApiData = apiData;
            Status = status;
        }

        public string ApiData { get; set; }

        public string Status { get; set; }

        public static string FromApiResponse(ApiResponse response, string name, string key, string successMessage = null, string errorMessage = null)
        {
            return GenericApiViewModel.FromFields(response?.Data, name, key, response?.LastUpdateTime.ToString(), successMessage, errorMessage);
        }

        public static string FromFields(string data, string name, string key, string lastUpdateTime = null, string successMessage = null, string errorMessage = null)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                var status = ApiStatus.Serialize(name, key, errorMessage ?? "API response was null", null, ApiState.Error);
                var viewModel = new GenericApiViewModel(null, status);
                return Helpers.Serialize(viewModel);
            }
            else
            {
                var status = ApiStatus.Serialize(name, key, successMessage ?? "Api success", lastUpdateTime, ApiState.Good);
                var viewModel = new GenericApiViewModel(data, status);
                return Helpers.Serialize(viewModel);
            }
        }
    }
}
