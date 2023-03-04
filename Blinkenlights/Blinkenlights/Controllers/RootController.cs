using Blinkenlights.Models;
using Blinkenlights.Models.Api.ApiHandler;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace Blinkenlights.Controllers
{
    public class RootController : Controller
    {
        private readonly ILogger<RootController> _logger;
		private readonly IConfiguration config;
		private readonly IApiHandler apiHandler;

		public RootController(IApiHandler apiHandler, ILogger<RootController> logger, IConfiguration config)
        {
            _logger = logger;
			this.config = config;
			this.apiHandler = apiHandler;
		}

        public IActionResult Index()
        {
            if (this.apiHandler.CheckForInvalidSecrets(out var invalidSecrets))
            {
                var errorModel = new ErrorViewModel()
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    MissingSecrets = invalidSecrets
                };
                return View("Error", errorModel);
            }

            return View(new IndexViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}