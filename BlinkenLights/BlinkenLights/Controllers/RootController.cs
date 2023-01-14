using BlinkenLights.Models;
using BlinkenLights.Models.WorldClock;
using BlinkenLights.Models.WWII;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System.Configuration;
using System.Diagnostics;

namespace BlinkenLights.Controllers
{
    public class RootController : Controller
    {
        private readonly IWebHostEnvironment Environment;
        private readonly ILogger<RootController> _logger;
        private readonly IConfiguration config;

        public RootController(ILogger<RootController> logger, IWebHostEnvironment environment, IConfiguration config)
        {
            _logger = logger;
            Environment = environment;
            this.config = config;
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel());
        }

        public IActionResult GetWorldClockModule()
        {
            string path = Path.Combine(this.Environment.WebRootPath, "DataSources", "TimeZoneInfo.json"); 
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
            string path = Path.Combine(this.Environment.WebRootPath, "DataSources", "WWII_DayByDay.json");
            var stringData = System.IO.File.ReadAllText(path);
            WWIIJsonModel wWIIViewModel = null;
            try
            {
                wWIIViewModel = JsonConvert.DeserializeObject<WWIIJsonModel>(stringData);
            }
            catch (JsonException)
            {
                return PartialView("WWIIModule", null);
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


                return PartialView("WWIIModule", dayModel);
            }
            else
            {
                return PartialView("WWIIModule", null);
            }
        }

        public IActionResult GetWeatherModule()
        {
            return PartialView("Weather", null);
        }

        public IActionResult GetLife360Module()
        {
            return PartialView("Life360", null);
        }

        public IActionResult GetWikipediaModule()
        {
            return PartialView("Wikipedia", null);
        }

        public async Task<string> GetLife360LocationsAsync()
        {
            var authorizationToken = this.config["Life360:AuthorizationToken"];
            var circleId = this.config["Life360:CircleId"];
            var endpointUrl = $"https://www.life360.com/v3/circles/{circleId}/members";
            var client = new RestClient(endpointUrl);

            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {authorizationToken}");
            var response = await client.GetAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            var models = new List<Life360Model>();
            JObject content = JsonConvert.DeserializeObject<JObject>(response.Content);
            content.TryGetValue("members", out var members);
            foreach(JObject member in members)
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

            return null;
        }

        public async Task<string> GetWeatherData()
        {
            // Upload with:
            // dotnet user-secrets set "OpenWeatherMap:ServiceApiKey" "{Secret}"

            //var authorizationToken = this.config["OpenWeatherMap:ServiceApiKey"];
            //var endpointUrl = $"https://api.openweathermap.org/data/3.0/onecall?lat=47.43&lon=-122.33&appid={authorizationToken}&exclude=minutely,daily&units=imperial";
            //var client = new RestClient(endpointUrl);

            //var request = new RestRequest();
            //var response = await client.GetAsync(request);
            //if (response?.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrWhiteSpace(response?.Content))
            //{
            //    return null;
            //}

            //var weatherDataJson = response.Content;

            string path = Path.Combine(this.Environment.WebRootPath, "DataSources", "CachedWeather.json");
            var weatherDataJson = System.IO.File.ReadAllText(path);
            return weatherDataJson;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}