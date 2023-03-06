using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;
using Blinkenlights.Transformers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// TODO Stock/Currency graphs

namespace Blinkenlights.Controllers
{
    public class ModulesController : Controller
	{
		private readonly IApiHandler ApiHandler;
		private readonly IServiceProvider ServiceProvider;

		public ModulesController(IApiHandler apiHandler, IServiceProvider serviceProvider)
        {
            this.ApiHandler = apiHandler;
            this.ServiceProvider = serviceProvider;
		}

		private async Task<IModuleViewModel> GetViewModel<T>() where T : TransformerBase
		{
			var transformer = (TransformerBase)ActivatorUtilities.CreateInstance<T>(this.ServiceProvider);
			return await transformer.Transform();
		}

		public async Task<IActionResult> GetCalendarModule()
		{
            var viewModel = await GetViewModel<CalendarTransformer>();
			return PartialView("CalendarModule", viewModel);
		}

		public async Task<IActionResult> GetTimeModule()
		{
            var viewModel = await GetViewModel<TimeTransformer>();
            return PartialView("TimeModule", viewModel);
        }

		public async Task<IActionResult> GetWWIIModule()
		{
			var viewModel = await GetViewModel<WWIITransformer>();
            return PartialView("WWIIModule", viewModel);
        }

        public async Task<IActionResult> GetLife360Module()
		{
			var viewModel = await GetViewModel<Life360Transformer>();
			return PartialView("Life360Module", viewModel);
		}

        public async Task<IActionResult> GetWeatherData()
        {
			var viewModel = await GetViewModel<WeatherTransformer>();
			return PartialView("WeatherModule", viewModel);
		}

        public async Task<IActionResult> GetMehData()
		{
			var viewModel = await GetViewModel<MehTransformer>();
            return PartialView("MehModule", viewModel);
        }

        public async Task<IActionResult> GetHeadlinesModule()
		{
			var viewModel = await GetViewModel<HeadlinesTransformer>();
            return PartialView("HeadlinesModule", viewModel);
        }

		public async Task<IActionResult> GetAstronomyModule()
		{
			var viewModel = await GetViewModel<AstronomyTransformer>();
			return PartialView("AstronomyModule", viewModel);
		}

	}
}
