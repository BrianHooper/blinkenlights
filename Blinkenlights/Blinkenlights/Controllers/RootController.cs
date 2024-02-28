using Blinkenlights.Models;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Transformers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace Blinkenlights.Controllers
{
    public class RootController : BlinkenController
	{
		private readonly IApiHandler apiHandler;
		private readonly ILogger<RootController> Logger;

		public RootController(IApiHandler apiHandler, IServiceProvider serviceProvider, ILogger<RootController> logger) : base(serviceProvider)
        {
            this.apiHandler = apiHandler;
            this.Logger = logger;
        }

        public IActionResult Index()
        {
            if (this.apiHandler.CheckForInvalidSecrets(out var invalidSecrets))
            {
                var secretsStr = string.Join(", ", invalidSecrets);
                this.Logger.LogError($"Cannot start due to invalid secrets: {secretsStr}");
                var errorModel = new ErrorViewModel()
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    MissingSecrets = invalidSecrets
                };
                return View("Error", errorModel);
            }

            return GetView<IndexTransformer>("Index");
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