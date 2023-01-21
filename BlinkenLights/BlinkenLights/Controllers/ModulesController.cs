using BlinkenLights.Models.ApiCache;
using BlinkenLights.Models.Life360;
using BlinkenLights.Models.WWII;
using BlinkenLights.Modules.WorldClock;
using BlinkenLights.Transformers;
using BlinkenLights.Utilities;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

/**
 * Modules:
 *  DONE - WWII OTD 
 *  DONE - Weather
 *  DONE - World Clock / Countdown Timers
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

            var viewModel = WorldClockTransformer.GetWorldClockViewModel(this.WebHostEnvironment);
            return PartialView("WorldClockModule", viewModel);
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
            if (Helpers.TryGetSecret(this.config, "Life360:AuthorizationToken", out var authorizationToken) && Helpers.TryGetSecret(this.config, "Life360:CircleId", out var circleId))
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
                Helpers.TryGetSecret(this.config, "VisualCrossing:ServiceApiKey", out var authorizationToken)
                ? $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/98148?unitGroup=us&key={authorizationToken}&contentType=json"
                : null;

            return await this.ApiCache.GetAndUpdateApiValue("VisualCrossing", 0, apiEndpoint);
        }

        public async Task<string> GetMehData()
        {
            var apiEndpoint =
                Helpers.TryGetSecret(this.config, "Meh:ServiceApiKey", out var authorizationToken)
                ? $"https://meh.com/api/1/current.json?apikey={authorizationToken}"
                : null;

            return await this.ApiCache.GetAndUpdateApiValue("Meh", 120, apiEndpoint);
        }
        
        public async Task<string> GetWikipediaData()
        {
            return await this.ApiCache.GetAndUpdateApiValue("Wikipedia", 120, $"http://127.0.0.1:5000/wikipedia");
        }
    }
}
