using Blinkenlights.Models;
using Blinkenlights.Models.ApiCache;
using Blinkenlights.Models.ApiResult;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Models.Life360;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlinkenLights.Transformers
{
    public class Life360Transformer
    {
        private const ApiType apiType = ApiType.Life360;

        public static string GetGenericApiModel(ApiResponse apiResponse)
        {
            if (apiResponse == null)
            {
                var status = ApiStatus.Failed(apiType, null, "Response is null");
                return GenericApiViewModel.FromApiStatus(null, status);
            }

            var models = new List<Life360Model>();
            JObject content;

            try 
            {
                content = JsonConvert.DeserializeObject<JObject>(apiResponse.Data); 
            }
            catch (JsonException)
            {
                var status = ApiStatus.Failed(apiType, null, "Exception while deserializing API response");
                return GenericApiViewModel.FromApiStatus(null, status);
            }

            content.TryGetValue("members", out var members);
            foreach (JObject member in members)
            {
                if (member is null)
                {
                    continue;
                }
                JToken location;
                member.TryGetValue("firstName", out var firstName);
                member.TryGetValue("location", out location);
                var locationObj = location as JObject;

                if (locationObj is null)
                {
                    continue;
                }
                locationObj.TryGetValue("latitude", out var latitude);
                locationObj.TryGetValue("longitude", out var longitude);
                locationObj.TryGetValue("timestamp", out var timestamp);

                var model = Life360Model.Parse(firstName?.Value<string>(), timestamp?.Value<string>(), latitude?.Value<string>(), longitude?.Value<string>());
                if (model != null)
                {
                    models.Add(model);
                }
            }

            if (models.Any())
            {
                var viewModel = new Life360ViewModel(models);
                var viewModelStr = JsonConvert.SerializeObject(viewModel);
                var status = ApiStatus.Success(apiType, apiResponse);
                return GenericApiViewModel.FromApiStatus(viewModelStr, status);
            }
            else
            {
                var status = ApiStatus.Failed(apiType, apiResponse, "Models list was empty");
                return GenericApiViewModel.FromApiStatus(null, status);
            }
        }
    }
}
