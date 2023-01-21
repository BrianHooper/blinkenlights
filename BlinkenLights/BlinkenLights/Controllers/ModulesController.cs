using BlinkenLights.Models;
using BlinkenLights.Models.WorldClock;
using BlinkenLights.Models.WWII;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

/**
 * Modules:
 *  DONE - WWII OTD 
 *  DONE - Weather
 *  DONE - World Clock
 *  DONE - Countdown Timers
 *  DONE - Life360
 *  Google Calendar upcoming
 *  Upcoming rocket launches
 *  DONE - Meh.com item
 *  Wikipedia front page
 *  NYT front page
 *  Stock/Currency graphs
 */

namespace BlinkenLights.Controllers
{
    public class ModulesController : Controller
    {
        private readonly IWebHostEnvironment WebHostEnvironment;
        private readonly ILogger<RootController> _logger;
        private readonly IConfiguration config;
        private readonly ApiCache ApiCache;

        public ModulesController(ILogger<RootController> logger, IWebHostEnvironment environment, IConfiguration config)
        {
            _logger = logger;
            WebHostEnvironment = environment;
            this.config = config;
            this.ApiCache = new ApiCache(Path.Combine(this.WebHostEnvironment.WebRootPath, "DataSources", "ApiCache.json"));
        }

        public IActionResult GetWorldClockModule()
        {
            string path = Path.Combine(this.WebHostEnvironment.WebRootPath, "DataSources", "TimeZoneInfo.json");
            var stringData = System.IO.File.ReadAllText(path);
            WorldClockViewModel viewModel = null;
            try
            {
                viewModel = JsonConvert.DeserializeObject<WorldClockViewModel>(stringData);
            }
            catch (JsonException)
            {
                return null;
            }

            return PartialView("WorldClockModule", viewModel);
        }

        public IActionResult GetWWIIModule()
        {
            string partialViewName = "WWIIModule";

            string path = Path.Combine(this.WebHostEnvironment.WebRootPath, "DataSources", "WWII_DayByDay.json");
            var stringData = System.IO.File.ReadAllText(path);
            WWIIJsonModel wWIIViewModel = null;
            try
            {
                wWIIViewModel = JsonConvert.DeserializeObject<WWIIJsonModel>(stringData);
            }
            catch (JsonException)
            {
                return PartialView(partialViewName, null);
            }

            var date = DateTime.Now.AddYears(-80);
            var dateStr = date.ToString("d MMM yyyy");
            if (wWIIViewModel?.Days?.TryGetValue(dateStr, out var wWIIDayJsonModel) == true)
            {
                var dateFormatted = String.Format("{0} {1:MMMM}, {2:yyyy}", date.Day.Ordinalize(), date, date);
                var globalEvents = wWIIDayJsonModel.Events.FirstOrDefault(kv => string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).Value;
                var regionalEvents = wWIIDayJsonModel.Events.Where(kv => !string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase));

                var dayModel = new WWIIDayModel()
                {
                    Date = dateFormatted,
                    GlobalEvents = globalEvents,
                    RegionalEvents = regionalEvents
                };


                return PartialView(partialViewName, dayModel);
            }
            else
            {
                return PartialView(partialViewName, null);
            }
        }

        public async Task<string> GetLife360LocationsAsync()
        {
            var apiCacheKey = "Life360";
            string apiResponse;
            if (!this.ApiCache.TryGetCachedValue(apiCacheKey, 1, out apiResponse))
            { 
                if (!TryGetSecret("Life360:AuthorizationToken", out var authorizationToken) || !TryGetSecret("Life360:CircleId", out var circleId))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "Failed to get authorization tokens" } });
                }

                var endpointUrl = $"https://www.life360.com/v3/circles/{circleId}/members";
                var client = new RestClient(endpointUrl);

                var request = new RestRequest();
                request.AddHeader("Authorization", $"Bearer {authorizationToken}");
                var response = await client.GetAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(response.Content))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "API response is invalid" } });
                }

                apiResponse = response.Content;
                this.ApiCache.TryUpdateCache(apiCacheKey, apiResponse);
            }

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
                var serialized = JsonConvert.SerializeObject(models);
                return serialized;
            }

            return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "Failed to build valid response" } });
        }

        public async Task<string> GetWeatherData()
        {
            var apiCacheKey = "VisualCrossing";
            string apiResponse;
            if (!this.ApiCache.TryGetCachedValue(apiCacheKey, 120, out apiResponse))
            {
                if (!TryGetSecret("VisualCrossing:ServiceApiKey", out var authorizationToken))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "Failed to get API secret" } });
                }
                var endpointUrl = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/98148?unitGroup=us&key={authorizationToken}&contentType=json";
                var client = new RestClient(endpointUrl);

                var request = new RestRequest();
                var response = await client.GetAsync(request);
                if (response?.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "API response is invalid" } });
                }

                apiResponse = response.Content;
                this.ApiCache.TryUpdateCache(apiCacheKey, apiResponse);
            }
            return apiResponse;
        }

        public async Task<string> GetMehData()
        {
            var apiCacheKey = "Meh";
            string apiResponse;
            if (!this.ApiCache.TryGetCachedValue(apiCacheKey, 120, out apiResponse))
            {
                if (!TryGetSecret("Meh:ServiceApiKey", out var authorizationToken))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "Failed to get API secret" } });
                }
                var endpointUrl = $"https://meh.com/api/1/current.json?apikey={authorizationToken}";
                var client = new RestClient(endpointUrl);

                var request = new RestRequest();
                var response = await client.GetAsync(request);
                if (response?.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "API response is invalid" } });
                }

                apiResponse = response.Content;
                this.ApiCache.TryUpdateCache(apiCacheKey, apiResponse);
            }
            return apiResponse;
        }
        
        public async Task<string> GetWikipediaData()
        {
            var apiCacheKey = "Wikipedia";
            string apiResponse;
            if (!this.ApiCache.TryGetCachedValue(apiCacheKey, 120, out apiResponse))
            {
                apiResponse = await CallApi($"http://127.0.0.1:5000/wikipedia");
                if (string.IsNullOrWhiteSpace(apiResponse))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", "API response is invalid" } });
                }

                this.ApiCache.TryUpdateCache(apiCacheKey, apiResponse);
            }
            return apiResponse;
        }

        private async Task<string> CallApi(string endpointUrl)
        {
            var client = new RestClient(endpointUrl);

            var request = new RestRequest();
            var response = await client.GetAsync(request);
            if (response?.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
            {
                return null;
            }

            return response.Content;
        }

        private bool TryGetSecret(string key, out string secret)
        {
            // Upload with:
            // dotnet user-secrets set "OpenWeatherMap:ServiceApiKey" "{Secret}"
            if (string.IsNullOrWhiteSpace(key))
            {
                secret = null;
                return false;
            }

            secret = this.config[key];
            if (string.IsNullOrWhiteSpace(secret))
            {
                secret = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
            }

            return !string.IsNullOrWhiteSpace(secret);
        }
    }
}
