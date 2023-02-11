using BlinkenLights.Models.Life360;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlinkenLights.Transformers
{
    public class Life360Transformer
    {
        public static bool TryGetLife360Model(string apiResponse, out string viewModel)
        {
            var models = new List<Life360Model>();
            JObject content = JsonConvert.DeserializeObject<JObject>(apiResponse);
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
                viewModel = JsonConvert.SerializeObject(models);
                return true;
            }
            else
            {
                viewModel = null;
                return false;
            }

        }
    }
}
