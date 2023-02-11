using BlinkenLights.Models.ApiCache;
using BlinkenLights.Transformers; 
using BlinkenLights.Utilities;

using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;

using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json.Serialization;

// Test branch policy 4

/**
 * Modules:
 *  DONE - WWII OTD 
 *  DONE - Weather
 *  DONE - World Clock / Countdown Timers
 *  DONE - Life360
 *  DONE - Meh.com item
 *  DONE - Wikipedia front page
 *  DONE - NYT front page
 *  Google Calendar upcoming
 *  Upcoming rocket launches
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

        public async Task<IActionResult> GetCalendarModule()
        {
            var apiResponse = await this.ApiCache.GetAndUpdateApiValue("GoogleCalendar", 60, null);
            return PartialView("CalendarModule");
        }

        public IActionResult GetTimeModule()
        {
            var viewModel = TimeTransformer.GetTimeViewModel(this.WebHostEnvironment);
            return PartialView("TimeModule", viewModel);
        }

        public IActionResult GetWWIIModule()
        {
            var viewModel = WWIITransformer.GetWWIIViewModel(this.WebHostEnvironment);
            return PartialView("WWIIModule", viewModel);
        }

        public async Task<string> GetLife360LocationsAsync()
        {
            var apiEndpoint = GetLife360ApiEndpoint(out var headers);
            var apiResponse = await this.ApiCache.GetAndUpdateApiValue("Life360", 5, apiEndpoint, headers);

            if (string.IsNullOrWhiteSpace(apiResponse))
            {
                return Helpers.ApiError("Failed to get valid API response");
            }

            if (Life360Transformer.TryGetLife360Model(apiResponse, out string viewModel)) 
            {
                return viewModel;
            }

            return Helpers.ApiError("Failed to build valid response");
        }

        private string GetLife360ApiEndpoint(out Dictionary<string, string> headers)
        {
            if (ApiCache.TryGetSecret(this.config, ApiSecret.Life360AuthorizationToken, out var authorizationToken) && ApiCache.TryGetSecret(this.config, ApiSecret.Life360CircleId, out var circleId))
            {
                headers = new Dictionary<string, string>()
                {
                    { "Authorization", $"Bearer {authorizationToken}" }
                };
                return $"https://www.life360.com/v3/circles/{circleId}/members";
            }
            headers = null;
            return null;
        }

        public async Task<string> GetWeatherData()
        {
            var apiEndpoint =
                ApiCache.TryGetSecret(this.config, ApiSecret.VisualCrossingServiceApiKey, out var authorizationToken)
                ? $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/98148?unitGroup=us&key={authorizationToken}&contentType=json"
                : null;

            return await this.ApiCache.GetAndUpdateApiValue("VisualCrossing", 60, apiEndpoint);
        }

        public async Task<string> GetMehData()
        {
            var apiEndpoint =
                ApiCache.TryGetSecret(this.config, ApiSecret.MehServiceApiKey, out var authorizationToken)
                ? $"https://meh.com/api/1/current.json?apikey={authorizationToken}"
                : null;

            return await this.ApiCache.GetAndUpdateApiValue("Meh", 120, apiEndpoint);
        }

        public async Task<IActionResult> GetHeadlinesModule()
        {
            var wikipediaData = await this.ApiCache.GetAndUpdateApiValue("Wikipedia", -1, $"http://127.0.0.1:5000/wikipedia");

            var nytApiEndpoint =
                ApiCache.TryGetSecret(this.config, ApiSecret.NewYorkTimesServiceApiKey, out var authorizationToken)
                ? $"https://api.nytimes.com/svc/topstories/v2/home.json?api-key={authorizationToken}"
                : null;

            var nytData = await this.ApiCache.GetAndUpdateApiValue("NewYorkTimes", 120, nytApiEndpoint);

            var headlinesViewModel = HeadlinesTransformer.GetHeadlinesViewModel(wikipediaData, nytData);
            return PartialView("HeadlinesModule", headlinesViewModel);
        }
    }
}
