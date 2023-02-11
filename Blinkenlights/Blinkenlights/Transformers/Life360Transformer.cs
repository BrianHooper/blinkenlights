using Blinkenlights.Models;
using Blinkenlights.Models.ApiCache;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Models.Life360;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlinkenLights.Transformers
{
    public class Life360Transformer
    {
        private const string ApiKey = "Life360";

        public static string GetGenericApiModel(ApiResponse apiResponse)
        {
            if (apiResponse == null)
            {
                return GenericApiViewModel.FromApiResponse(null, ApiKey, ApiKey, errorMessage: "API response is null");
            }

            var models = new List<Life360Model>();
            JObject content;

            try 
            {
                content = JsonConvert.DeserializeObject<JObject>(apiResponse.Data); 
            }
            catch (JsonException)
            {
                return GenericApiViewModel.FromApiResponse(null, ApiKey, ApiKey, errorMessage: "Exception while deserializing API response");
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
                return GenericApiViewModel.FromFields(viewModelStr, ApiKey, ApiKey);
            }
            else
            {
                return GenericApiViewModel.FromApiResponse(null, ApiKey, ApiKey, errorMessage: "Models list was empty");
            }
        }
    }
}
