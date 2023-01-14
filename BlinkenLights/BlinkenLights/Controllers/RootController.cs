using BlinkenLights.Models;
using Microsoft.AspNetCore.Mvc;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}